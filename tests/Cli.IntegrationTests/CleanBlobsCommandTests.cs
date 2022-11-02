using BlobCleaner.IntegrationTests.Utils;
using Cli.IntegrationTests.Utils;
using FluentAssertions;
using Xunit;

namespace BlobCleaner.IntegrationTests;

public class CleanBlobsCommandTests : TestBase
{
    private readonly string[] containers =
    {
        "simulation", "simulation-1", "space-viewer", "optimization", "results", "no-to-remove"
    };

    [Theory, Conventions]
    public async Task ShouldCleanBlob_WithoutForce(CliRunner runner, CaptureConsoleOutput output)
    {
        await CheckCleanContainer(runner, output, false, 1);
    }

    [Theory, Conventions]
    public async Task ShouldCleanBlob_WithForce(CliRunner runner, CaptureConsoleOutput output)
    {
        await CheckCleanContainer(runner, output, true, 0);
    }

    private async Task CheckCleanContainer(CliRunner runner, CaptureConsoleOutput output, bool force,
        int expectedCount)
    {
        // Arrange
        await CreateContainers(containers);

        // Act
        runner.Run(CreateParams(force, true));
        output.Add("End tests");


        var count = await Client.BlobContainersCount();

        // Assert
        count.Should().Be(expectedCount);
    }

    private static string[] CreateParams(bool force, bool verbose) => new[]
    {
        "azure", "clean", "-c", ConnectionString, "-f", force.ToString(), "-v", verbose.ToString()
    };
}
