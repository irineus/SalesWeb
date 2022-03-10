namespace SalesWeb.Models.Interfaces
{
    public interface IFileStorage
    {
        void UploadFile(string fileName, string sourcePath);
        void DownloadFile(string fileName, string destinationPath);
    }
}
