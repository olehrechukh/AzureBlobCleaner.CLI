using BlobCleaner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var app = ConsoleApp
    .CreateBuilder(args, options =>
    {
        options.NameConverter = input => input.ToLowerInvariant();
        options.ShowDefaultCommand = true;
    })
    .ConfigureLogging(builder => builder.SetMinimumLevel(LogLevel.Information))
    .ConfigureServices((context, services) => services.AddTransient<CleanBlobCommandHandler>())
    .Build();

app.AddCommand("clean", "Clean azure blob", ClearBlob);
app.AddCommand("get", "Get azure blob information",
    (ILogger<ConsoleApp> logger) => { logger.LogInformation("Not yet implemented"); });

app.Run();

app.Logger.LogInformation("End app");

async Task ClearBlob(
    CleanBlobCommandHandler cleaner,
    ConsoleAppContext ctx,
    [Option("c", "Connection string")] string connectionString,
    [Option("f", "Clean full blob if true")] bool force = false,
    [Option("v", "Set output to verbose messages")] bool verbose = false)
{
    await cleaner.Handle(new CleanBlobCommand(connectionString, force, verbose), ctx.CancellationToken);
}

app.Logger.LogInformation("End");
return 1;
