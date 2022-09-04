using System.Runtime.CompilerServices;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;

namespace BlobCleaner;

public record CleanBlobCommand(string ConnectionString, bool Force, bool Verbose);

public class CleanBlobCommandHandler
{
    private readonly ILogger<CleanBlobCommandHandler> logger;

    private readonly string[] resultPrefixes = {"simulation", "space-viewer", "optimization", "results"};

    public CleanBlobCommandHandler(ILogger<CleanBlobCommandHandler> logger)
    {
        this.logger = logger;
    }

    public async Task Handle(CleanBlobCommand command, CancellationToken cancellationToken)
    {
        var (connectionString, force, verbose) = command;
        var blobServiceClient = new BlobServiceClient(connectionString);

        var prefixes = GetPrefixes(force);

        await Task.WhenAll(prefixes.Select(prefix => Clear(blobServiceClient, prefix, verbose, cancellationToken)));
    }

    private async Task Clear(BlobServiceClient blobServiceClient, string prefix, bool verbose,
        CancellationToken cancellationToken)
    {
        var tasks = await blobServiceClient.ListContainerNames(prefix, null, cancellationToken)
            .Select(container => DeleteContainer(blobServiceClient, container, verbose, cancellationToken))
            .ToListAsync(cancellationToken);

        await Task.WhenAll(tasks);
    }

    private async Task DeleteContainer(BlobServiceClient blobServiceClient, string containerName, bool verbose,
        CancellationToken cancellationToken)
    {
        await blobServiceClient.DeleteBlobContainerAsync(containerName, cancellationToken: cancellationToken);

        if (verbose)
        {
            logger.LogInformation("Deleted container {name}", containerName);
        }
    }

    private IEnumerable<string> GetPrefixes(bool force) => force ? new string[] {null} : resultPrefixes;
}

public static class BlobServiceClientExtensions
{
    public static async IAsyncEnumerable<string> ListContainerNames(this BlobServiceClient blobServiceClient,
        string prefix, int? segmentSize, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // Call the listing operation and enumerate the result segment.
        var resultSegment =
            blobServiceClient.GetBlobContainersAsync(BlobContainerTraits.Metadata, prefix, cancellationToken)
                .AsPages(default, segmentSize);

        await foreach (var containerPage in resultSegment.WithCancellation(cancellationToken))
        {
            foreach (var containerItem in containerPage.Values)
            {
                yield return containerItem.Name;
            }
        }
    }
}
