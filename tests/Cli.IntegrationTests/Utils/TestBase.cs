using Xunit;

namespace BlobCleaner.IntegrationTests.Utils;

public class TestBase : IAsyncLifetime
{
    protected readonly BlobClient Client;

    protected const string ConnectionString =
        "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";

    protected TestBase() => Client = new BlobClient(ConnectionString);

    public async Task InitializeAsync() => await Client.ClearAll();

    protected Task CreateContainers(IEnumerable<string> names)
    {
        var tasks = names.Select(x => Client.CreateBlobContainer(x));

        return Task.WhenAll(tasks);
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
