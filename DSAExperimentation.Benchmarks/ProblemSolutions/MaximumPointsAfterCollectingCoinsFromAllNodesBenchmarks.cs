using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumPointsAfterCollectingCoinsFromAllNodes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// MaximumPointsAfterCollectingCoinsFromAllNodesSolution's, the same methods
// MaximumPointsAfterCollectingCoinsFromAllNodesTests proves correct. A skewed chain,
// not a bushy random tree - the same worst-case shape
// LongestPathWithDifferentAdjacentCharactersBenchmarks' Setup comment gives for a
// tree-fold benchmark, since both arms here already share the same O(n * MaxHalvings)
// complexity and what this actually separates is per-node overhead: a
// Dictionary<(int,int),long> memo lookup per call vs. CoinPointsAlgebra.Combine's
// flat array indexing.
[MemoryDiagnoser]
public class MaximumPointsAfterCollectingCoinsFromAllNodesBenchmarks
{
    private const int RandomSeed = 2920; // LeetCode problem number
    private const int MaxCoinsExclusive = 10_000;
    private const int K = 2;

    [Params(200, 2_000)]
    public int NodeCount;

    private int[][] _edges = null!;
    private int[] _coins = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _edges = new int[NodeCount - 1][];
        for (var i = 1; i < NodeCount; i++)
        {
            _edges[i - 1] = [i - 1, i];
        }

        _coins = Enumerable.Range(0, NodeCount).Select(_ => random.Next(0, MaxCoinsExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long MemoizedRecursion() => MaximumPointsAfterCollectingCoinsFromAllNodesSolution.MaxPointsByMemoizedRecursion(_edges, _coins, K);

    [Benchmark]
    public long TreeFold() => MaximumPointsAfterCollectingCoinsFromAllNodesSolution.MaxPointsByTreeFold(_edges, _coins, K);
}
