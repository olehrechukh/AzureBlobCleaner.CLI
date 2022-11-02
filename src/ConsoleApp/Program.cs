// See https://aka.ms/new-console-template for more information

using Application.Commands;
using Microsoft.Extensions.Logging;

using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

var dbCleaner = new CleanDbCommandHandler(loggerFactory.CreateLogger<CleanDbCommandHandler>());
var blobCleaner = new CleanBlobCommandHandler(loggerFactory.CreateLogger<CleanBlobCommandHandler>());

var mySqlConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__MySql");
var blobConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__AzureBlob");

Console.WriteLine(mySqlConnectionString);
Console.WriteLine(blobConnectionString);

await dbCleaner.Handle(new CleanDbCommand(mySqlConnectionString, true), CancellationToken.None);
await blobCleaner.Handle(new CleanBlobCommand(blobConnectionString, true, true), CancellationToken.None);

Console.WriteLine("Finished");
