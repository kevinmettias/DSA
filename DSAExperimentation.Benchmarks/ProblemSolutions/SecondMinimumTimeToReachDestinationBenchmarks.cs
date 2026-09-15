using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SecondMinimumTimeToReachDestination;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SecondMinimumTimeToReachDestinationSolution's, the same
// methods SecondMinimumTimeToReachDestinationTests proves correct - the same
// dual-distance BFS over two different frontiers. On this problem's near-single-cycle
// graph (m == n, per LC 2045's own generation constraint) the walk only ever touches
// O(n) roads in total, so the List arm's O(n) shift per dequeue is precisely what
// turns it quadratic where the Queue arm stays linear.
//
// Each arm is handed the prepared IntersectionNetwork its hoisted overload takes, so
// reading LeetCode's [a, b] rows into an adjacency list is charged to [GlobalSetup]
// rather than to the walk being measured.
[MemoryDiagnoser]
public class SecondMinimumTimeToReachDestinationBenchmarks
{
    private const int Time = 3;
    private const int Change = 5;

    private IntersectionNetwork _network = null!;

    [Params(200, 2_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        // A single cycle over every intersection (m == NodeCount roads), matching LC
        // 2045's own generation guarantee and giving every intersection a genuine
        // second route - the long way around. LeetCode numbers intersections from 1,
        // so every row is one past its index.
        var edges = new int[NodeCount][];

        for (var i = 0; i < NodeCount; i++)
        {
            edges[i] = [i + 1, ((i + 1) % NodeCount) + 1];
        }

        _network = IntersectionNetwork.Build(NodeCount, edges);
    }

    [Benchmark(Baseline = true)]
    public int ListFrontierBfs() =>
        SecondMinimumTimeToReachDestinationSolution.SecondMinimumTimeByListFrontier(_network, Time, Change);

    [Benchmark]
    public int QueueFrontierBfs() =>
        SecondMinimumTimeToReachDestinationSolution.SecondMinimumTimeByQueueFrontier(_network, Time, Change);
}
