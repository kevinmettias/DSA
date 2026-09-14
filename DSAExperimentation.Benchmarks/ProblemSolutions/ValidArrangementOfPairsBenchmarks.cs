using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ValidArrangementOfPairs;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ValidArrangementOfPairsSolution's, the same methods
// ValidArrangementOfPairsTests proves correct - the BCL Dictionary<int, List<int>>
// Hierholzer walk against the same walk over this repo's own
// HashMap<int, Stack<int>>. Both are handed LeetCode's own input shape, generated
// once in [GlobalSetup] so pair construction is not charged to the measured method.
[MemoryDiagnoser]
public class ValidArrangementOfPairsBenchmarks
{
    private const int NodeCount = 64;
    private const int RandomSeed = 2097;

    [Params(200, 2_000)]
    public int PairCount;

    private int[][] _pairs = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _pairs = new int[PairCount][];

        for (var i = 0; i < PairCount; i++)
        {
            _pairs[i] = [random.Next(0, NodeCount), random.Next(0, NodeCount)];
        }
    }

    [Benchmark(Baseline = true)]
    public int[][] DictionaryWithList() =>
        ValidArrangementOfPairsSolution.ValidArrangementByDictionaryWithList(_pairs);

    [Benchmark]
    public int[][] RepoHashMapWithStack() =>
        ValidArrangementOfPairsSolution.ValidArrangementByRepoHashMapWithStack(_pairs);
}
