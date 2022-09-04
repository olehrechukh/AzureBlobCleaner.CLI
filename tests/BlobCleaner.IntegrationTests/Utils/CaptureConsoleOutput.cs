using System.Globalization;
using System.Text;

namespace BlobCleaner.IntegrationTests.Utils;

public class CaptureConsoleOutput : IDisposable
{
    private readonly TextWriter originalWriter;
    private readonly StringBuilder stringBuilder;

    public CaptureConsoleOutput()
    {
        originalWriter = Console.Out;

        stringBuilder = new StringBuilder();
        var stringWriter = new StringWriter(stringBuilder, CultureInfo.InvariantCulture);
        Console.SetOut(stringWriter);
    }

    public void Add(string message)
    {
        stringBuilder.AppendLine(message);
    }

    public string[] GetLogs() => stringBuilder
        .ToString()
        .Split(Environment.NewLine)
        .Where(log => !string.IsNullOrEmpty(log))
        .ToArray();

    public void Dispose() => Console.SetOut(originalWriter);
}
