using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Respawn;

namespace BlobCleaner.Commands;

public record CleanDbCommand(string ConnectionString, bool Verbose);

internal sealed class CleanDbCommandHandler
{
    private readonly ILogger<CleanDbCommandHandler> logger;


    public CleanDbCommandHandler(ILogger<CleanDbCommandHandler> logger) => this.logger = logger;

    public async Task Handle(CleanDbCommand command, CancellationToken cancellationToken)
    {
        var (connectionString, verbose) = command;

        var database = new MySqlConnectionStringBuilder(connectionString).Database;
        var checkpoint = GetCheckpoint(database);

        if (verbose)
        {
            logger.LogInformation("Started db cleaning for {name} db", database);
        }

        await using var mySqlConnection = new MySqlConnection(connectionString);
        await mySqlConnection.OpenAsync(cancellationToken);
        await checkpoint.Reset(mySqlConnection);

        if (verbose)
        {
            logger.LogInformation("Ended db cleaning for {name} db", database);
        }
    }

    private static Checkpoint GetCheckpoint(string database)
    {
        var checkpoint = new Checkpoint
        {
            TablesToIgnore = new[] {"__EFMigrationsHistory"},
            DbAdapter = DbAdapter.MySql,
            SchemasToInclude = new[] {database}
        };
        return checkpoint;
    }
}
