namespace eCommerce.Tests.Helpers
{
    public class FakeFileHelper : IFileHelper
    {
        public Task<string> SaveImageAsync(IFormFile file, string rootPath, string folderName)
        {
            return Task.FromResult("fakepath.jpg");
        }
        public void DeleteImage(string rootPath, string relativePath)
        {
            throw new NotImplementedException();
        }
    }
}
