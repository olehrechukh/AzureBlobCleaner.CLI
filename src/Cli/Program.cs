using Application.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var app = ConsoleApp
    .CreateBuilder(args, options =>
    {
        options.NameConverter = input => input.ToLowerInvariant();
        options.ShowDefaultCommand = true;
    })
    .ConfigureLogging(builder => builder.SetMinimumLevel(LogLevel.Information))
    .ConfigureServices((_, services) =>
    {
        services.AddTransient<CleanBlobCommandHandler>();
        services.AddTransient<CleanDbCommandHandler>();
    })
    .Build();

app.AddSubCommand("azure", "clean", "Clean azure blob", ClearBlob);
app.AddSubCommand("db", "clean", "Clean database", ClearDb);
app.AddCommand("get", "Get azure blob information",
    (ILogger<ConsoleApp> logger) => { logger.LogInformation("Not yet implemented"); });

app.Run();

async Task ClearBlob(
    CleanBlobCommandHandler cleaner,
    ConsoleAppContext ctx,
    [Option("c", "Connection string")] string connectionString,
    [Option("f", "Clean full blob if true")] bool force = false,
    [Option("v", "Set output to verbose messages")] bool verbose = false)
{
    await cleaner.Handle(new CleanBlobCommand(connectionString, force, verbose), ctx.CancellationToken);
}

async Task ClearDb(
    CleanDbCommandHandler cleaner,
    ConsoleAppContext ctx,
    [Option("c", "Connection string")] string connectionString,
    [Option("v", "Set output to verbose messages")] bool verbose = false)
{
    await cleaner.Handle(new CleanDbCommand(connectionString, verbose), ctx.CancellationToken);
}

return 1;
