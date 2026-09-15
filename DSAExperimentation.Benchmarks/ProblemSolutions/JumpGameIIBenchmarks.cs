using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.JumpGameII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Jump Game II (LC 45): the textbook O(n) greedy two-pointer scan vs. modeling the
// same reachability as an implicit unweighted-hop graph (index i -> every index one
// jump away) and answering it with this repo's own ShortestPath.Dijkstra. Jump
// distances are capped at 10 so the implicit graph stays sparse (O(n) edges, not
// O(n^2)) - the point of the comparison is the primitive composition, not stress
// testing Dijkstra on a dense graph.
//
// Harness only: both arms are JumpGameIISolution's, the same methods
// JumpGameIITests proves correct. The hop graph is built once in [GlobalSetup] so
// its construction isn't charged to the search being measured.
[MemoryDiagnoser]
public class JumpGameIIBenchmarks
{
    private const int MaxJumpDistanceExclusive = 11;

    private int[] _jumps = [];

    private HopNode[] _hopGraph = [];
    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _jumps = new int[Length];

        for (var i = 0; i < Length - 1; i++)
        {
            _jumps[i] = random.Next(1, MaxJumpDistanceExclusive);
        }

        _hopGraph = JumpGameIISolution.BuildHopGraph(_jumps);
    }

    [Benchmark(Baseline = true)]
    public int GreedyTwoPointer() => JumpGameIISolution.MinJumpsByGreedyTwoPointer(_jumps);

    [Benchmark]
    public int DijkstraOverHopGraph() => JumpGameIISolution.MinJumpsByDijkstraOverHopGraph(_hopGraph);
}
