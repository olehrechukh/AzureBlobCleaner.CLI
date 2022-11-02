using AutoFixture;
using AutoFixture.Xunit2;
using Cli.IntegrationTests.Utils;

namespace BlobCleaner.IntegrationTests.Utils;

public class ConventionsAttribute : AutoDataAttribute
{
    public ConventionsAttribute() : base(Create)
    {
    }

    private static IFixture Create()
    {
        var fixture = new Fixture();

        fixture.Inject(new CaptureConsoleOutput());
        fixture.Inject(new CliRunner());

        return fixture;
    }
}
