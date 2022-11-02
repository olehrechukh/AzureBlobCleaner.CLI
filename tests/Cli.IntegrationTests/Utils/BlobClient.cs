using Application.Commands;
using Azure.Storage.Blobs;

namespace BlobCleaner.IntegrationTests.Utils;

public class BlobClient
{
    private readonly BlobServiceClient blobServiceClient;

    public BlobClient(string connectionString) => blobServiceClient = new BlobServiceClient(connectionString);

    public async Task<BlobContainerClient> CreateBlobContainer(string name,
        CancellationToken cancellationToken = default)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(name);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        return containerClient;
    }

    public async Task ClearAll()
    {
        var tasks = await blobServiceClient.ListContainerNames(null, null)
            .Select(container => blobServiceClient.DeleteBlobContainerAsync(container)).Select(dummy => (Task)dummy)
            .ToListAsync();

        await Task.WhenAll(tasks);
    }


    public async Task<List<string>> AllBlobContainers() =>
        await blobServiceClient.ListContainerNames(null, null).ToListAsync();

    public async Task<int> BlobContainersCount() => await blobServiceClient.ListContainerNames(null, null).CountAsync();
}
