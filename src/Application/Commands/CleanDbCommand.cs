using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace Application.Commands;

public record CleanDbCommand(string ConnectionString, bool Verbose);

public sealed class CleanDbCommandHandler
{
    private readonly ILogger<CleanDbCommandHandler> logger;

    public CleanDbCommandHandler(ILogger<CleanDbCommandHandler> logger) => this.logger = logger;

    public async Task Handle(CleanDbCommand command, CancellationToken cancellationToken)
    {
        var (connectionString, verbose) = command;

        var database = new MySqlConnectionStringBuilder(connectionString).Database;

        if (verbose)
        {
            logger.LogInformation("Started db cleaning for {name} db", database);
        }

        await using var mySqlConnection = new MySqlConnection(connectionString);
        await mySqlConnection.OpenAsync(cancellationToken);

        foreach (var sqlCommand in SqlCommands)
        {
            var mySqlCommand = mySqlConnection.CreateCommand();
            mySqlCommand.CommandText = sqlCommand;

            var query = await mySqlCommand.ExecuteNonQueryAsync(cancellationToken);

            if (verbose)
            {
                logger.LogInformation("Executed command '{c}' with result '{r}'", sqlCommand, query);
            }
        }


        if (verbose)
        {
            logger.LogInformation("Ended db cleaning for {name} db", database);
        }
    }

    private readonly List<string> SqlCommands = new()
    {
        "update rs_workspaces set Calculation=\'{\"Elements\":[]}\'",
        "update rs_workspaces set CalculationStatus=0",
        "delete from SimulationRuns",
        "delete from SimulationRunConsumers",
        "delete from ProcessDataViewerTransformationRuns",
        "delete from ProcessDataViewerTransformationConsumers",
        "delete from SpaceViewerRuns",
        "delete from SpaceViewerResultRunConsumers",
        "delete from SpaceViewerTransformationRuns",
        "delete from SpaceViewerTransformationRunConsumers",
        "delete from OptimizationRuns",
        "delete from OptimizationRunConsumers",
        "delete from OptimizationRunTableTransformationRun",
        "delete from OptimizationRunProcessDataViewerTransformationRun",
        "delete from OptimizationTransformationRuns",
        "delete from OptimizationTransformationRunConsumers",
        "delete from ProcessDataViewerTransformationRunSimulationRun",
        "delete from SimulationRunTableTransformationRun",
        "delete from TableTransformationConsumers",
        "delete from TableTransformationRuns"
    };
}
