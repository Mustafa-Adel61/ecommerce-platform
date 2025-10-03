namespace eCommerce.Application.Common
{
    public interface IFileHelper
    {
        Task<string> SaveImageAsync(IFormFile file, string rootPath, string folderName);
        void DeleteImage(string rootPath, string relativePath);
    }
}
