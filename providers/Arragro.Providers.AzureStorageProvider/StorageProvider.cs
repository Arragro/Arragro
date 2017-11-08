﻿using Arragro.Common.CacheProvider;
using Arragro.Common.Interfaces.Providers;
using Arragro.Common.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Arragro.Providers.AzureStorageProvider
{
    public class StorageProvider<FolderIdType, FileIdType> : IStorageProvider<FolderIdType, FileIdType>
    {
        private readonly IImageProvider _imageService;

        private readonly string _storageConnectionString;
        private readonly int _cacheControlMaxAge;

        private readonly CloudStorageAccount _account;
        private readonly CloudBlobClient _client;
        private readonly CloudBlobContainer _assetContainer;

        public StorageProvider(
            IImageProvider imageProcessor,
            string storageConnectionString,
            int cacheControlMaxAge = 0)
        {
            _imageService = imageProcessor;
            _storageConnectionString = storageConnectionString;
            _cacheControlMaxAge = cacheControlMaxAge;
             _account = CloudStorageAccount.Parse(_storageConnectionString);
            _client = _account.CreateCloudBlobClient();

            _assetContainer = _client.GetContainerReference("assets");
            _assetContainer.CreateIfNotExistsAsync().Wait();
        }

        const string THUMBNAIL_ASSETKEY = "ThumbNail:";
        const string ASSET_ASSETKEY = "Asset:";
        const string ASSET_QUALITY_ASSETKEY = "Asset:Quality:";
        const string ASSET_QUALITY_WIDTH_ASSETKEY = "Asset:Quality:Width:";

        public async Task<bool> Delete(FolderIdType folderId, FileIdType fileId, bool thumbNail = false)
        {
            var thumbNails = thumbNail ? "thumbnails/" : "";
            var blob = _assetContainer.GetBlobReference($"{folderId}/{thumbNails}{fileId}");
            return await blob.DeleteIfExistsAsync();
        }

        private async Task DeleteFolder(string folder)
        {
            var directory = _assetContainer.GetDirectoryReference(folder);

            BlobContinuationToken continuationToken = null;
            CancellationToken cancellationToken = new CancellationToken();
            BlobResultSegment resultSegment = null;

            do
            {
                resultSegment = await directory.ListBlobsSegmentedAsync(true, BlobListingDetails.All, 10, continuationToken, null, null, cancellationToken);
                foreach (CloudBlob blobItem in resultSegment.Results)
                {
                    await blobItem.DeleteIfExistsAsync();
                }
                continuationToken = resultSegment.ContinuationToken;
            }
            while (continuationToken != null);
        }

        public async Task Delete(FolderIdType folderId)
        {
            await DeleteFolder($"{folderId}/thumbnails");
            await DeleteFolder($"{folderId}");
        }

        private async Task<Uri> Get(FolderIdType folderId, FileIdType fileId)
        {
            var key = $"{ASSET_ASSETKEY}{folderId}:{fileId}";
            var cacheItem = CacheProviderManager.CacheProvider.Get<Uri>(key);
            if (cacheItem != null && cacheItem.Item != null)
                return cacheItem.Item;

            CloudBlob blob;

            blob = _assetContainer.GetBlobReference($"{folderId}/{fileId}");

            if (await blob.ExistsAsync())
            {
                CacheProviderManager.CacheProvider.Set(key, blob.Uri, new Arragro.Common.CacheProvider.CacheSettings(new TimeSpan(0, 30, 0), true));
                return blob.Uri;
            }

            return null;
        }

        private async Task<Uri> Upload(FolderIdType folderId, FileIdType fileId, byte[] data, string mimeType)
        {
            var blob = _assetContainer.GetBlockBlobReference($"{folderId}/{fileId}");
            using (var stream = new MemoryStream(data))
            {
                blob.Properties.ContentType = mimeType;
                await blob.UploadFromStreamAsync(stream);
                return blob.Uri;
            }
        }

        private async Task<Uri> GetImageThumbnail(FolderIdType folderId, FileIdType fileId)
        {
            var key = $"{THUMBNAIL_ASSETKEY}{folderId}:{fileId}";
            var cacheItem = CacheProviderManager.CacheProvider.Get<Uri>(key);
            if (cacheItem != null && cacheItem.Item != null)
                return cacheItem.Item;

            CloudBlob blob = _assetContainer.GetBlobReference($"{folderId}/thumbnails/{fileId}");
            if (await blob.ExistsAsync())
            {
                CacheProviderManager.CacheProvider.Set(key, blob.Uri, new Arragro.Common.CacheProvider.CacheSettings(new TimeSpan(0, 30, 0), true));
                return blob.Uri;
            }

            return null;
        }

        public async Task ResetCacheControl()
        {
            var blobs = await _assetContainer.ListBlobsSegmentedAsync((BlobContinuationToken)null);
            do
            {
                foreach (IListBlobItem blob in blobs.Results)
                {
                    await this.ResetCloudBlobCacheControl(blob, _cacheControlMaxAge);
                }
                blobs = await this._assetContainer.ListBlobsSegmentedAsync(blobs.ContinuationToken);
            }
            while (blobs.ContinuationToken != null);
        }

        private async Task ResetCloudBlobCacheControl(IListBlobItem blobItem, int cacheControlMaxAge)
        {
            if (blobItem is CloudBlockBlob)
            {
                CloudBlockBlob blob = blobItem as CloudBlockBlob;
                blob.Properties.CacheControl = string.Format("public, max-age={0}", cacheControlMaxAge);
                await blob.SetPropertiesAsync();
            }
            else
            {
                var blobDirectory = blobItem as CloudBlobDirectory;
                var blobs = await this._assetContainer.ListBlobsSegmentedAsync(blobDirectory.Prefix, null);
                do
                {
                    foreach (IListBlobItem blob in blobs.Results)
                    {
                        await ResetCloudBlobCacheControl(blob, cacheControlMaxAge);
                    }
                    blobs = await this._assetContainer.ListBlobsSegmentedAsync(blobDirectory.Prefix, blobs.ContinuationToken);
                }
                while (blobs.ContinuationToken != null);
            }
        }

        private async Task<Uri> UploadThumbnail(FolderIdType folderId, FileIdType fileId, byte[] data, string mimeType)
        {
            var blob = _assetContainer.GetBlockBlobReference($"{folderId}/thumbnails/{fileId}");
            using (var stream = new MemoryStream(data))
            {
                blob.Properties.ContentType = mimeType;
                await blob.UploadFromStreamAsync(stream);
                return blob.Uri;
            }
        }

        public async Task<Uri> Get(FolderIdType folderId, FileIdType fileId, bool thumbnail = false)
        {
            if (thumbnail)
                return await GetImageThumbnail(folderId, fileId);
            return await Get(folderId, fileId);
        }

        public async Task<CreateImageFromImageResult> CreateImageFromExistingImage(FolderIdType folderId, FileIdType fileId, FileIdType newFileId, int quality, int width, bool asProgressive = true)
        {
            var fileName =$"{folderId}/{fileId}";
            var newFileName =$"{folderId}/{newFileId}";

            var blobCopy = _assetContainer.GetBlobReference(newFileName);
            if (!await blobCopy.ExistsAsync())
            {
                CloudBlockBlob blob = _assetContainer.GetBlockBlobReference(fileName);

                if (await blob.ExistsAsync())
                {
                    byte[] bytes;

                    using (var ms = new MemoryStream())
                    {
                        await blob.DownloadToStreamAsync(ms);
                        bytes = ms.ToArray();
                    }

                    var imageResult = _imageService.GetImage(bytes, width, quality, asProgressive);
                    var uri = await Upload(folderId, newFileId, imageResult.Bytes, blob.Properties.ContentType);
                    var thumbnailUri = await Upload(folderId, newFileId, imageResult.Bytes, blob.Properties.ContentType, true);

                    return new CreateImageFromImageResult
                    {
                        ImageProcessResult = imageResult,
                        Uri = uri,
                        ThumbnailUri = thumbnailUri
                    };
                }
                else
                {
                    throw new Exception($"The blob you want to copy doesn't exists - {blob.Uri}!");
                }
            }
            else
            {
                throw new Exception($"The blob you want to create already exists - {blobCopy.Uri}!");
            }
        }

        public async Task<Uri> Upload(FolderIdType folderId, FileIdType fileId, byte[] data, string mimeType, bool thumbnail = false)
        {
            if (thumbnail)
                return await UploadThumbnail(folderId, fileId, data, mimeType);
            return await Upload(folderId, fileId, data, mimeType);
        }

        public async Task<Uri> Rename(FolderIdType folderId, FileIdType fileId, FileIdType newFileId, bool thumbnail = false)
        {
            var fileName = thumbnail ? $"{folderId}/thumbnails/{fileId}" : $"{folderId}/{fileId}";
            var newFileName = thumbnail ? $"{folderId}/thumbnails/{newFileId}" : $"{folderId}/{newFileId}";

            var blobCopy = _assetContainer.GetBlobReference(newFileName);
            if (!await blobCopy.ExistsAsync())
            {
                CloudBlockBlob blob = _assetContainer.GetBlockBlobReference(fileName);

                if (await blob.ExistsAsync())
                {
                    await blobCopy.StartCopyAsync(blob.Uri);
                    await blob.DeleteIfExistsAsync();
                }
            }
            return blobCopy.Uri;
        }
    }
}
