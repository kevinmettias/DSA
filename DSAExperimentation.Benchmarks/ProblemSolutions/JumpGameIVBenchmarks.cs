using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.JumpGameIV;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Jump Game IV (LC 1345): the textbook BFS directly over the array - a Queue<int>,
// a same-value index map, and the standard "clear the group once every member has
// been enqueued" trick that keeps it from re-scanning a large equal-value run on
// every visit - vs. modeling the exact same i+1/i-1/same-value reachability as an
// implicit unweighted-hop graph and answering it with this repo's own
// ShortestPath.Dijkstra. Values are drawn from a range five times narrower than
// Length so equal-value groups stay small (dense enough to matter, not so dense the
// hop graph's edge count explodes past O(n)).
//
// Harness only: both arms are JumpGameIVSolution's, the same methods
// JumpGameIVTests proves correct. The hop graph is built once in [GlobalSetup] so
// its construction isn't charged to the search being measured.
[MemoryDiagnoser]
public class JumpGameIVBenchmarks
{
    private const int RandomSeed = 1345; // LeetCode problem number

    private const int ValueRangeDivisor = 5;

    private int[] _arr = [];

    private ValueHopNode[] _hopGraph = [];
    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _arr = new int[Length];

        for (var i = 0; i < Length; i++)
        {
            var upperBoundExclusive = Math.Max(1, Length / ValueRangeDivisor);
            _arr[i] = random.Next(0, upperBoundExclusive);
        }

        _hopGraph = JumpGameIVSolution.BuildHopGraph(_arr);
    }

    [Benchmark(Baseline = true)]
    public int BfsWithGroupPruning() => JumpGameIVSolution.MinJumpsByBfsWithGroupPruning(_arr);

    [Benchmark]
    public int DijkstraOverHopGraph() => JumpGameIVSolution.MinJumpsByDijkstraOverHopGraph(_hopGraph);
}
