using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountValidPathsInATree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountValidPathsInATreeSolution's, the same methods
// CountValidPathsInATreeTests proves correct. The per-pair path walk runs one BFS
// per unordered node pair - O(n) per pair over O(n^2) pairs - against this repo's
// own DisjointSet, which splits the tree into non-prime blobs in O(n a(n)) and then
// sweeps each prime node's arms once. A genuine complexity split, not a
// constant-factor difference.
//
// Both arms take LeetCode's own edges[][] input, so there is nothing to hoist: the
// only thing [GlobalSetup] prepares is the workload itself.
[MemoryDiagnoser]
public class CountValidPathsInATreeBenchmarks
{
    // LC problem number, reused as the deterministic tree seed.
    private const int TreeSeed = 1;

    [Params(50, 200)]
    public int NodeCount;

    private int[][] _edges = null!;

    // A random recursive tree (parent[i] uniform in [1, i)), the same shape
    // MinimumHeightTreesBenchmarks/LongestPathWithDifferentAdjacentCharacters
    // Benchmarks already build their own random test trees from.
    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(TreeSeed);
        _edges = new int[NodeCount - 1][];

        for (var i = 2; i <= NodeCount; i++)
        {
            _edges[i - 2] = [random.Next(1, i), i];
        }
    }

    [Benchmark(Baseline = true)]
    public long PerPairPathWalk() =>
        CountValidPathsInATreeSolution.CountValidPathsByPerPairPathWalk(NodeCount, _edges);

    [Benchmark]
    public long DisjointSetBlobs() =>
        CountValidPathsInATreeSolution.CountValidPathsByDisjointSetBlobs(NodeCount, _edges);
}
