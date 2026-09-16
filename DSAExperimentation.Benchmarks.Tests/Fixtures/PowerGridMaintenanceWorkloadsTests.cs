using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for PowerGridMaintenanceWorkloads (ARCHITECTURE 17.7). The reading depends on LC
// 3607's grid being one connected graph spanning every station, with a query script that takes stations
// offline often enough that most maintenance checks have to search their grid for a replacement.
public sealed partial class PowerGridMaintenanceWorkloadsTests
{
    private const int StationCount = 200;
    private const int QueryCount = 1_000;
    private const int Seed = 3607;
    private const int ExtraConnectionsPerStation = 2;
    private const int FirstStation = 1;
    private const int SecondStation = FirstStation + 1;
    private const int LastStation = StationCount;
    private const int ConnectionFieldCount = 2;
    private const int QueryFieldCount = 2;
    private const int OfflineQueryKind = 2;
    private const int MaintenanceQueryKind = 1;
    private const int OfflineEveryNthQuery = 3;

    [Fact]
    public void BuildConnections_EveryConnection_IsATwoStationPairInsideTheGrid()
    {
        var connections = PowerGridMaintenanceWorkloads.BuildConnections(StationCount, Seed);

        Assert.All(connections, connection => Assert.Equal(ConnectionFieldCount, connection.Length));
        Assert.All(connections, connection => Assert.InRange(connection[0], FirstStation, LastStation));
        Assert.All(connections, connection => Assert.InRange(connection[1], FirstStation, LastStation));
        Assert.All(connections, connection => Assert.NotEqual(connection[0], connection[1]));
    }

    // The spanning edge the generator writes for every station is what makes the grid connected, which
    // is the property the maintenance checks are searching around.
    [Fact]
    public void BuildConnections_EveryStationBeyondTheFirst_GetsABackEdgeToAnEarlierStation()
    {
        var connections = PowerGridMaintenanceWorkloads.BuildConnections(StationCount, Seed);

        foreach (var station in Enumerable.Range(SecondStation, StationCount - FirstStation))
        {
            Assert.Contains(connections, connection => connection[1] == station && connection[0] < station);
        }
    }

    [Fact]
    public void BuildConnections_ConnectionCount_StaysBetweenTheSpanningBackboneAndTheDensityCap()
    {
        var connections = PowerGridMaintenanceWorkloads.BuildConnections(StationCount, Seed);

        Assert.InRange(
            connections.Length,
            StationCount - FirstStation,
            (StationCount - FirstStation) + (StationCount * ExtraConnectionsPerStation));
    }

    [Fact]
    public void BuildConnections_SameSeed_ReturnsTheSameConnections() =>
        Assert.Equal(
            AnswerText.Of(PowerGridMaintenanceWorkloads.BuildConnections(StationCount, Seed)),
            AnswerText.Of(PowerGridMaintenanceWorkloads.BuildConnections(StationCount, Seed)));

    [Fact]
    public void BuildQueries_QueryCount_ReturnsOneQueryPerPositionWithTwoFields()
    {
        var queries = PowerGridMaintenanceWorkloads.BuildQueries(StationCount, QueryCount, Seed);

        Assert.Equal(QueryCount, queries.Length);
        Assert.All(queries, query => Assert.Equal(QueryFieldCount, query.Length));
    }

    [Fact]
    public void BuildQueries_EveryQuery_NamesAStationInRangeWithOneOfTheTwoQueryKinds()
    {
        var queries = PowerGridMaintenanceWorkloads.BuildQueries(StationCount, QueryCount, Seed);

        Assert.All(
            queries,
            query => Assert.True(
                query[0] == OfflineQueryKind || query[0] == MaintenanceQueryKind));
        Assert.All(
            queries,
            query => Assert.InRange(query[1], FirstStation, LastStation));
    }

    // Every third query takes a station offline and the rest are maintenance checks on a random station,
    // which is what makes most checks eventually land on an offline one.
    [Fact]
    public void BuildQueries_EveryThirdQuery_TakesAStationOfflineAndTheRestAreMaintenanceChecks()
    {
        var queries = PowerGridMaintenanceWorkloads.BuildQueries(StationCount, QueryCount, Seed);

        Assert.All(
            Enumerable.Range(0, QueryCount),
            index => Assert.Equal(
                index % OfflineEveryNthQuery == 0 ? OfflineQueryKind : MaintenanceQueryKind,
                queries[index][0]));
    }

    [Fact]
    public void BuildQueries_SameSeed_ReturnsTheSameQueries() =>
        Assert.Equal(
            AnswerText.Of(PowerGridMaintenanceWorkloads.BuildQueries(StationCount, QueryCount, Seed)),
            AnswerText.Of(PowerGridMaintenanceWorkloads.BuildQueries(StationCount, QueryCount, Seed)));
}
