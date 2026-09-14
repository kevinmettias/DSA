using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfWaysToArriveAtDestination;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfWaysToArriveAtDestinationSolution's, the same
// methods NumberOfWaysToArriveAtDestinationTests proves correct. Each is handed the
// prepared WaysGraph its hoisted overload takes, so the shared Dijkstra distance
// labelling is charged to [GlobalSetup] and the measured difference stays purely
// how the shortest-time journeys are counted afterwards.
[MemoryDiagnoser]
public class NumberOfWaysToArriveAtDestinationBenchmarks
{
    // The network's "step 2" roads skip one intersection (u to u + 2) with a
    // matching travel time of 2.
    private const int TwoStepOffset = 2;

    // Kept modest (<=30), same reasoning as
    // NumberOfRestrictedPathsFromFirstToLastNodeBenchmarks: NaiveDfs's blowup here
    // really is O(golden-ratio^N).
    [Params(20, 30)]
    public int N;

    private WaysGraph _graph = null!;

    [GlobalSetup]
    public void Setup() => _graph = WaysGraph.Build(NodeCount(N), BuildTwoStepRoads(N));

    // The chain spans stepCount time-1 roads, so it has one more intersection than
    // steps.
    private static int NodeCount(int stepCount) => stepCount + 1;

    // Intersection u gets a time-1 road to u + 1 and a time-2 road to u + 2, both
    // equally shortest, so every intersection's distance to the destination is
    // exactly its remaining step count and the same downstream intersection is
    // reached two ways at every layer - the Fibonacci recurrence that makes the
    // unmemoized arm's recount genuinely exponential.
    private static int[][] BuildTwoStepRoads(int stepCount)
    {
        var roads = new List<int[]>();

        for (var u = 0; u < stepCount; u++)
        {
            roads.Add([u, u + 1, 1]);
        }

        for (var u = 0; u + TwoStepOffset <= stepCount; u++)
        {
            roads.Add([u, u + TwoStepOffset, TwoStepOffset]);
        }

        return [.. roads];
    }

    [Benchmark(Baseline = true)]
    public long NaiveDfs() => NumberOfWaysToArriveAtDestinationSolution.CountWaysByNaiveDfs(_graph);

    [Benchmark]
    public long DagFoldMemoized() => NumberOfWaysToArriveAtDestinationSolution.CountWaysByDagFold(_graph);
}
