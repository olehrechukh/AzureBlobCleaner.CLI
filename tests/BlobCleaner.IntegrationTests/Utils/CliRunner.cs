using System.Reflection;
using Microsoft.Extensions.Logging;

namespace BlobCleaner.IntegrationTests.Utils;

public class CliRunner
{
    private readonly MethodInfo mainMethod;

    public CliRunner()
    {
        var programType = typeof(Program);

        var method = programType.GetMethod("<Main>$", BindingFlags.Static | BindingFlags.NonPublic);

        mainMethod = method ?? throw new InvalidProgramException();
    }

    public void Run(string arguments)
    {
        Run(arguments.Split(" "));
    }

    public void Run(string[] arguments)
    {
        mainMethod.Invoke(null, new object[] {arguments});
    }
}
