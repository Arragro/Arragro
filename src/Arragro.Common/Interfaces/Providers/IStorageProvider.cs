using System;
using System.Threading.Tasks;

namespace Arragro.Common.Interfaces.Providers
{
    public interface IStorageProvider<FolderIdType, FileIdType> 
    {
        Task<bool> Delete(FolderIdType folderId, FileIdType fileId, bool thumbNail = false);
        Task Delete(FolderIdType folderId);
        Task<Uri> Get(FolderIdType folderId, FileIdType fileId);
        Task<Uri> GetImageThumbnail(FolderIdType folderId, FileIdType fileId);
        Task<Uri> GetImage(FolderIdType folderId, FileIdType fileId, int quality, int width, bool canCreate = false);
        Task<Uri> Upload(FolderIdType folderId, FileIdType fileId, byte[] data, string mimeType);
        Task<Uri> UploadThumbnail(FolderIdType folderId, FileIdType fileId, byte[] data, string mimeType);
        Task ResetCacheControl();
    }
}
