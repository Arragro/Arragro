using Arragro.Common.CacheProvider;
using Arragro.Common.Interfaces.Providers;
using Arragro.Common.Models;
using Arragro.Providers.InMemoryStorageProvider.FileInfo;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Arragro.Providers.InMemoryStorageProvider
{
    public class StorageProvider<FolderIdType, FileIdType> : IStorageProvider<FolderIdType, FileIdType>
    {
        private readonly IImageProvider _imageService;
        private readonly InMemoryFileProvider _provider;

        public StorageProvider(
            IImageProvider imageProcessor)
        {
            _imageService = imageProcessor;
            _provider = new InMemoryFileProvider();
        }

        const string THUMBNAIL_ASSETKEY = "ThumbNail:";
        const string ASSET_ASSETKEY = "Asset:";
        const string ASSET_QUALITY_ASSETKEY = "Asset:Quality:";
        const string ASSET_QUALITY_WIDTH_ASSETKEY = "Asset:Quality:Width:";

        public void ClearCache()
        {
            Cache.RemoveFromCache($"{THUMBNAIL_ASSETKEY}.*", true);
            Cache.RemoveFromCache($"{ASSET_ASSETKEY}.*", true);
            Cache.RemoveFromCache($"{ASSET_QUALITY_ASSETKEY}.*", true);
            Cache.RemoveFromCache($"{ASSET_QUALITY_WIDTH_ASSETKEY}.*", true);
        }

        public async Task<bool> Delete(FolderIdType folderId, FileIdType fileId, bool thumbNail = false)
        {
            return await Task.Run(() =>
            {
                var thumbNails = thumbNail ? "thumbnails/" : "";
                var fileInfo = _provider.Directory.GetFile($"/{folderId}/{thumbNails}{fileId}");
                if (fileInfo == null)
                    return false;

                fileInfo.Delete();
                return true;
            });
        }

        private async Task DeleteFolder(string folder)
        {
            await Task.Run(() =>
            {
                var directory = _provider.Directory.GetFolder(folder);
                directory.Delete();
            });
        }

        public async Task Delete(FolderIdType folderId)
        {
            await DeleteFolder($"/{folderId}/thumbnails");
            await DeleteFolder($"/{folderId}");
        }

        public async Task<Uri> Get(FolderIdType folderId, FileIdType fileId)
        {
            var fileInfo = _provider.GetFileInfo($"/{folderId}/{fileId}");

            if (fileInfo != null)
            {
                return await Task.Run(() =>
                {
                    return new Uri($"http://inmemoryfileprovider.com/{folderId}/{fileId}");
                });
            }

            return null;
        }

        public async Task<Uri> GetImageThumbnail(FolderIdType folderId, FileIdType fileId)
        {
            var fileInfo = _provider.GetFileInfo($"/{folderId}/thumbnails/{fileId}");

            if (fileInfo != null)
            {
                return await Task.Run(() =>
                {
                    return new Uri($"http://inmemoryfileprovider.com/{folderId}/{fileId}");
                });
            }

            return null;
        }

        public async Task<Uri> GetImage(FolderIdType folderId, FileIdType fileId, int quality, int width, bool canCreate = false)
        {
            var qualityPath = $"/{quality}/";
            var widthPath = $"{width}";

            var fileInfo = _provider.GetFileInfo($"/{folderId}/{fileId}{qualityPath}{widthPath}");

            if (fileInfo != null)
                return new Uri($"http://inmemoryfileprovider.com/{folderId}/{fileId}");
            else
                if (await GetImageBytes(folderId, fileId, quality, width, canCreate) != null)
                    return new Uri($"http://inmemoryfileprovider.com/{folderId}/{fileId}");
            return null;
        }

        private async Task<byte[]> CreateImageIfNotExists(FolderIdType folderId, FileIdType fileId, int quality, int width)
        {
            var fileInfo = _provider.GetFileInfo($"/{folderId}/{fileId}");

            if (fileInfo == null)
            {
                using (var ms = new MemoryStream()) 
                {
                    await fileInfo.CreateReadStream().CopyToAsync(ms);
                    var data = ms.ToArray();

                    ImageProcessResult result = _imageService.GetImage(data, width, quality, true);

                    if (result != null)
                    {
                        await Upload(folderId, fileId, quality, width, result.Bytes, "mime-type-in-memory");
                        return result.Bytes;
                    }
                }
            }
            return null;
        }

        private async Task<byte[]> GetImageBytes(FolderIdType folderId, FileIdType fileId, int quality, int width, bool canCreate = false)
        {
            var qualityPath = $"/{quality}/";
            var widthPath = $"{width}";
                        
            var fileInfo = _provider.GetFileInfo($"{folderId}/{fileId}{qualityPath}{widthPath}");

            if (fileInfo == null)
            {
                if (canCreate)
                {
                    return await CreateImageIfNotExists(folderId, fileId, quality, width);
                }
                return null;
            }

            using (var ms = new MemoryStream())
            {
                fileInfo.CreateReadStream().CopyTo(ms);
                return ms.ToArray();
            }
        }

        public async Task Upload(FolderIdType folderId, FileIdType fileId, byte[] data, string mimeType)
        {
            await Task.Run(() =>
            {
                using (var ms = new MemoryStream(data))
                {
                    ms.Position = 0;
                    _provider.Directory.AddFile($"/{folderId}", new MemoryStreamFileInfo(ms, Encoding.UTF8, fileId.ToString()));
                }
            });
        }

        private async Task Upload(FolderIdType folderId, FileIdType fileId, int quality, int width, byte[] data, string mimeType)
        {
            await Task.Run(() =>
            {
                using (var ms = new MemoryStream(data))
                {
                    ms.Position = 0;
                    _provider.Directory.AddFile($"/{folderId}/{fileId}/{quality}", new MemoryStreamFileInfo(ms, Encoding.UTF8, width.ToString()));
                }
            });
        }

        public async Task UploadThumbnail(FolderIdType folderId, FileIdType fileId, byte[] data, string mimeType)
        {
            await Task.Run(() =>
            {
                using (var ms = new MemoryStream(data))
                {
                    ms.Position = 0;
                    _provider.Directory.AddFile($"/{folderId}/thumbnails", new MemoryStreamFileInfo(ms, Encoding.UTF8, fileId.ToString()));
                }
            });
        }

        private async Task DeleteBlob(Uri uri)
        {
            await Task.Run(() =>
            {
                var filePath = uri.ToString().Replace("http://inmemoryfileprovider.com", "");
                var fileInfo = _provider.Directory.GetFile(filePath);
                if (fileInfo != null)
                    fileInfo.Delete();
            });
        }

        private bool Contains(string source, string toCheck, StringComparison comp)
        {
            return source != null && toCheck != null && source.IndexOf(toCheck, comp) >= 0;
        }
        
        public async Task ResetCacheControl()
        {
            await Task.Run(() =>
            {
                return;
            });
        }
    }
}
