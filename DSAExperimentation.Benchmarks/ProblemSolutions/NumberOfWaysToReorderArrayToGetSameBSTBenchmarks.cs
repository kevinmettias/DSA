using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfWaysToReorderArrayToGetSameBST;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfWaysToReorderArrayToGetSameBSTSolution's, the
// same methods NumberOfWaysToReorderArrayToGetSameBSTTests proves correct. The
// workload is a deterministic random permutation of 1..Length, so the BST both arms
// describe is a balanced-on-average one; [GlobalSetup] owns the shuffle, leaving
// each measured call to do only its own counting work.
[MemoryDiagnoser]
public class NumberOfWaysToReorderArrayToGetSameBSTBenchmarks
{
    // LC problem number, reused as the deterministic permutation seed.
    private const int RandomSeed = 1569;

    private int[] _nums = [];

    [Params(200, 1000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(1, Length).OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int NaiveListSplitting() =>
        NumberOfWaysToReorderArrayToGetSameBSTSolution.CountWaysByListSplitting(_nums);

    [Benchmark]
    public int PrimitiveComposed() =>
        NumberOfWaysToReorderArrayToGetSameBSTSolution.CountWaysByTreeFold(_nums);
}
