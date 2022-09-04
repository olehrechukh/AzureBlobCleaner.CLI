using Xunit;

namespace BlobCleaner.IntegrationTests.Utils;

public class TestBase : IAsyncLifetime
{
    protected readonly BlobClient Client;
    protected readonly string ConnectionString;

    protected TestBase()
    {
        ConnectionString = Environment.GetEnvironmentVariable(EnvironmentVariables.BlobConnectionString) ??
                           throw new ArgumentNullException(nameof(EnvironmentVariables.BlobConnectionString));

        Client = new BlobClient(ConnectionString);
    }

    public async Task InitializeAsync() => await Client.ClearAll();

    protected Task CreateContainers(IEnumerable<string> names)
    {
        var tasks = names.Select(x => Client.CreateBlobContainer(x));

        return Task.WhenAll(tasks);
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
