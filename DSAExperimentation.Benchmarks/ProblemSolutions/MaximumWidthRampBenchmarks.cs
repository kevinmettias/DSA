using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumWidthRamp;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumWidthRampSolution's, the same methods
// MaximumWidthRampTests proves correct. The O(n^2) pairwise scan is the baseline the
// O(n) candidate stack has to beat.
[MemoryDiagnoser]
public class MaximumWidthRampBenchmarks
{
    private const int RandomSeed = 962; private int[] _nums = [];

    // LC problem number

    [Params(500, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(0, Length)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int PairwiseScan() => MaximumWidthRampSolution.MaxWidthRampByPairwiseScan(_nums);

    [Benchmark]
    public int CandidateStack() => MaximumWidthRampSolution.MaxWidthRampByCandidateStack(_nums);
}
