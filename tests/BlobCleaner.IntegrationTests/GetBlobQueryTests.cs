using BlobCleaner.IntegrationTests.Utils;
using FluentAssertions;
using Xunit;

namespace BlobCleaner.IntegrationTests;

public class GetBlobQueryTests
{
    [Theory, Conventions]
    public void ShouldReceiveNotImplemented(CliRunner runner, CaptureConsoleOutput output)
    {
        // Arrange
        runner.Run("get");

        // Act
        var logs = output.GetLogs();

        // Assert
        logs.Should().Contain("Not yet implemented");
    }
}
