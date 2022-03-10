using Azure.Storage.Blobs;
using SalesWeb.Models.Interfaces;

namespace SalesWeb.Services
{
    public class SellerImgStorageService : IFileStorage
    {
        private readonly IConfiguration _configuration;
        private readonly BlobContainerClient _container;

        public SellerImgStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
            _container = new BlobContainerClient(_configuration["AZURE-STORAGE-CONNECTION-STRING"],"sellers-img") ;
        }

        public void UploadFile(string fileName, string sourcePath)
        {
            throw new NotImplementedException();
        }

        public async void DownloadFile(string fileName, string destinationPath)
        {
            BlobClient blob = _container.GetBlobClient(fileName);
            await blob.DownloadToAsync(destinationPath + "\\" + fileName);
        }
    }
}
