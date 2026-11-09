using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace RaceDay.API.Services;

public class BlobStorageService
{
    private readonly IConfiguration _configuration;

    public BlobStorageService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string> UploadAsync(IFormFile file)
    {
        var connectionString =
            _configuration["BlobStorage:ConnectionString"];

        var containerName =
            _configuration["BlobStorage:ContainerName"];

        if (string.IsNullOrWhiteSpace(connectionString) ||
            string.IsNullOrWhiteSpace(containerName))
        {
            throw new InvalidOperationException(
                "Azure Blob Storage is not configured yet.");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        var allowedExtensions =
            new[] { ".jpg", ".jpeg", ".png", ".webp" };

        if (!allowedExtensions.Contains(extension))
            throw new InvalidOperationException("Unsupported image format.");

        if (file.Length == 0 || file.Length > 5 * 1024 * 1024)
            throw new InvalidOperationException(
                "Image must be between 1 byte and 5 MB.");

        var blobService = new BlobServiceClient(connectionString);
        var container = blobService.GetBlobContainerClient(containerName);

        var fileName = $"{Guid.NewGuid()}{extension}";
        var blob = container.GetBlobClient(fileName);

        await using var stream = file.OpenReadStream();

        await blob.UploadAsync(stream, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = file.ContentType
            }
        });

        return blob.Uri.ToString();
    }
}