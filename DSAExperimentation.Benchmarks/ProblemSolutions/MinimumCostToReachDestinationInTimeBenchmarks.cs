using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MinimumCostToReachDestinationInTime;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumCostToReachDestinationInTimeSolution's, the
// same methods MinimumCostToReachDestinationInTimeTests proves correct - the
// textbook unmemoized walk over every route the minute budget can pay for
// (exponential in the city count) against this repo's ShortestPath.Dijkstra run
// over the (city, elapsedTime) expansion of the same map, whose node count is only
// cities x budget. Each arm is handed the prepared input its hoisted overload
// takes - an adjacency RoadNetwork for the walk, a built TimeCityGraph for
// Dijkstra - so construction is charged to [GlobalSetup] rather than to the search
// being measured.
//
// LastCity is the last city index, so the map holds LastCity + 1 cities. The sizes are
// smaller than the pre-migration ones (20 and 30) because the baseline changed: it used
// to walk a one-way chain of its own, and now walks LeetCode's genuinely two-way roads,
// which branch four ways instead of two. That is the arm's real cost, and 14 already
// puts it in the millions of calls.
[MemoryDiagnoser]
public class MinimumCostToReachDestinationInTimeBenchmarks
{
    private RoadNetwork _roads = null!;

    private TimeCityGraph _graph = null!;
    private int _maxTime;
    [Params(10, 14)]
    public int LastCity { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var edges = TimedRoadWorkloads.BuildStepChain(LastCity);
        var passingFees = TimedRoadWorkloads.BuildCyclingFees(LastCity);

        _maxTime = TimedRoadWorkloads.BudgetFor(LastCity);
        _roads = RoadNetwork.Build(edges, passingFees);
        _graph = TimeCityGraph.Build(_maxTime, edges, passingFees);
    }

    [Benchmark(Baseline = true)]
    public int NaiveDfs() =>
        MinimumCostToReachDestinationInTimeSolution.MinCostByNaiveDfs(_roads, _maxTime);

    [Benchmark]
    public int StateExpandedDijkstra() =>
        MinimumCostToReachDestinationInTimeSolution.MinCostByStateExpandedDijkstra(_graph);
}
