using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CountOfRangeSum;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountOfRangeSumSolution's, the same methods
// CountOfRangeSumSolutionTests proves correct. [GlobalSetup] only sizes and seeds the raw
// nums array - LeetCode's own input shape - so each strategy still does its own
// prefix-sum, coordinate-compression and sweep work under measurement.
public class CountOfRangeSumBenchmarks
{
    private const int Lower = -1_000;
    private const int Upper = 1_000;
    private const int RandomSeed = 327; // LC problem number
    private const int ValueRangeMagnitude = 100;

    private int[] _nums = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = SeededDraws.Values(Length, -ValueRangeMagnitude, ValueRangeMagnitude, random);
    }

    [Benchmark(Baseline = true)]
    public int PairwisePrefixScan() =>
        CountOfRangeSumSolution.CountByPairwisePrefixScan(_nums, Lower, Upper);

    [Benchmark]
    public int FenwickTreeSweep() =>
        CountOfRangeSumSolution.CountByFenwickSweep(_nums, Lower, Upper);
}
