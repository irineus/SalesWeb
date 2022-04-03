using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using SalesWeb.Models.Interfaces;
using SalesWeb.Services.Exceptions;

namespace SalesWeb.Services
{
    public class SellerImgStorageService : IFileStorage
    {
        private readonly IConfiguration _configuration;
        private readonly BlobServiceClient _blobServiceClient;
        //private readonly BlobContainerClient _container;

        public SellerImgStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
            _blobServiceClient = new BlobServiceClient(_configuration["AZURE-STORAGE-CONNECTION-STRING"]);
            //_container = new BlobContainerClient(_configuration["AZURE-STORAGE-CONNECTION-STRING"], "sellers-img");
        }

        public void UploadFile(string fileName, string sourcePath)
        {
            throw new NotImplementedException();
        }

        public async void DownloadFile(string fileName, string destinationPath)
        {
            var container = _blobServiceClient.GetBlobContainerClient("sellers-img");
            BlobClient blob = container.GetBlobClient(fileName);
            await blob.DownloadToAsync(destinationPath + "\\" + fileName);
        }

        public string GetBlobsByTag(string key, string value)
        {
            var blobTag = @"@container = 'sellers-img' AND """ + key + @""" = '" + value + "'";
            var blobs = new List<TaggedBlobItem>();
            foreach (TaggedBlobItem taggedBlobItem in _blobServiceClient.FindBlobsByTags(blobTag))
            {
                blobs.Add(taggedBlobItem);
            }
            if (blobs.Count == 0)
                throw new KeyNotFoundException("Image not found");
            if (blobs.Count > 1)
                throw new MultipleKeyException("Multiple images with this key were found");
            var tagvalue = blobs.FirstOrDefault().BlobName;
            return tagvalue;
        }
    }
}
