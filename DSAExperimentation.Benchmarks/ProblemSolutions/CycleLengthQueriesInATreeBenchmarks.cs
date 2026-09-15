using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CycleLengthQueriesInATree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CycleLengthQueriesInATreeSolution's, the same methods
// CycleLengthQueriesInATreeTests proves correct - a per-query ancestor Dictionary
// against the HeapArrayIndex.Parent two-pointer walk. Both are O(log id) per query
// (tree depth is bounded by n regardless of how many queries run), so the gap this
// benchmark demonstrates is allocation, not asymptotic complexity: MemoryDiagnoser
// should show the Dictionary-per-query baseline's Gen0/bytes grow with QueriesCount
// while the Parent-arithmetic arm allocates only the answer array. The queries
// themselves are generated once in [GlobalSetup].
[MemoryDiagnoser]
public class CycleLengthQueriesInATreeBenchmarks
{
    private const int RandomSeed = 2509; // LC problem number
    private const int TreeLevels = 20; private int[][] _queries = [];

    // ids range over [1, 2^20 - 1]

    [Params(1_000, 20_000)]
    public int QueriesCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var maxId = (1 << TreeLevels) - 1;

        _queries = Enumerable.Range(0, QueriesCount)
            .Select(_ => BuildDistinctPair(random, maxId))
            .ToArray();
    }

    private static int[] BuildDistinctPair(Random random, int maxId)
    {
        var a = random.Next(1, maxId + 1);
        int b;

        do
        {
            b = random.Next(1, maxId + 1);
        }
        while (b == a);

        return [a, b];
    }

    [Benchmark(Baseline = true)]
    public int[] AncestorDictionaryWalk() =>
        CycleLengthQueriesInATreeSolution.CycleLengthQueriesByAncestorDictionary(TreeLevels, _queries);

    [Benchmark]
    public int[] ParentIndexTwoPointerWalk() =>
        CycleLengthQueriesInATreeSolution.CycleLengthQueriesByParentIndexWalk(TreeLevels, _queries);
}
