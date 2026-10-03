using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MinimumCostToReachDestinationInTime;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumCostToReachDestinationInTimeSolution's, the
// same methods MinimumCostToReachDestinationInTimeSolutionTests proves correct - the
// textbook unmemoized walk over every route the minute budget can pay for
// (exponential in the city count) against this repo's ShortestPath.Dijkstra run
// over the (city, elapsedTime) expansion of the same map, whose node count is only
// cities x budget. Each arm is handed the prepared input its hoisted overload
// takes - an adjacency RoadNetwork for the walk, a built TimeCityGraph for
// Dijkstra - so construction is charged to [GlobalSetup] rather than to the search
// being measured.
//
// Sizes are per arm, each the last city index, so the map holds one more city than
// that. The walk stops at 14: LeetCode's roads are two-way, so it branches four ways
// and 14 already puts it in the millions of calls. Dijkstra runs on to 400, inside
// LC 1928's bound of 1,000 roads, which this two-roads-per-city chain reaches near 500.
public class MinimumCostToReachDestinationInTimeBenchmarks
{
    private Dictionary<int, PreparedMap> _mapsBySize = [];

    public static IEnumerable<int> BaselineSizes => [10, 14];

    public static IEnumerable<int> DijkstraSizes => [.. BaselineSizes, 100, 400];

    // Every size any arm runs is built here, outside the timed region; an arm looks its own up.
    [GlobalSetup]
    public void Setup() => _mapsBySize = DijkstraSizes.ToDictionary(lastCity => lastCity, BuildMap);

    private static PreparedMap BuildMap(int lastCity)
    {
        var edges = TimedRoadWorkloads.BuildStepChain(lastCity);
        var passingFees = TimedRoadWorkloads.BuildCyclingFees(lastCity);
        var maxTime = TimedRoadWorkloads.BudgetFor(lastCity);

        return new PreparedMap(
            RoadNetwork.Build(edges, passingFees), TimeCityGraph.Build(maxTime, edges, passingFees), maxTime);
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BaselineSizes))]
    public int NaiveDfs(int lastCity)
    {
        var map = _mapsBySize[lastCity];

        return MinimumCostToReachDestinationInTimeSolution.MinCostByNaiveDfs(map.Roads, map.MaxTime);
    }

    [Benchmark]
    [ArgumentsSource(nameof(DijkstraSizes))]
    public int StateExpandedDijkstra(int lastCity) =>
        MinimumCostToReachDestinationInTimeSolution.MinCostByStateExpandedDijkstra(_mapsBySize[lastCity].Graph);

    // One size's map in both shapes the arms take, with the minute budget the walk is held to.
    private readonly record struct PreparedMap(RoadNetwork Roads, TimeCityGraph Graph, int MaxTime);
}
