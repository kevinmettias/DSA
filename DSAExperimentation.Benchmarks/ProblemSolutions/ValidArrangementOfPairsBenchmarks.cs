using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.ValidArrangementOfPairs;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ValidArrangementOfPairsSolution's, the same methods
// ValidArrangementOfPairsSolutionTests proves correct - the BCL Dictionary<int, List<int>>
// Hierholzer walk against the same walk over this repo's own
// HashMap<int, Stack<int>>. Both are handed LeetCode's own input shape, generated
// once in [GlobalSetup] so pair construction is not charged to the measured method.
// ValidArrangementOfPairsWorkloads walks the pairs as one shuffled trail, so they are
// distinct, never join a node to itself, and have the valid arrangement LC 2097
// promises.
public class ValidArrangementOfPairsBenchmarks
{
    private const int NodeCount = 64;
    private const int RandomSeed = 2097;

    private int[][] _pairs = [];

    [Params(200, 2_000)]
    public int PairCount { get; set; }

    [GlobalSetup]
    public void Setup() => _pairs = ValidArrangementOfPairsWorkloads.BuildPairs(PairCount, NodeCount, RandomSeed);

    [Benchmark(Baseline = true)]
    public int[][] DictionaryWithList() =>
        ValidArrangementOfPairsSolution.ValidArrangementByDictionaryWithList(_pairs);

    [Benchmark]
    public int[][] RepoHashMapWithStack() =>
        ValidArrangementOfPairsSolution.ValidArrangementByRepoHashMapWithStack(_pairs);
}
