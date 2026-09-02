using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MakeArrayElementsEqualToZero;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MakeArrayElementsEqualToZeroSolution's, the
// same methods MakeArrayElementsEqualToZeroTests proves correct. nums[0] is
// pinned to 0 to satisfy LC 3354's "at least one zero" precondition; the rest
// mixes zeros and small positive values so both strategies do real work.
[MemoryDiagnoser]
public class MakeArrayElementsEqualToZeroBenchmarks
{
    private const int Seed = 3354;
    private const int MaxValueExclusive = 20;
    private const double ZeroProbability = 0.3;

    [Params(20, 100)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = new int[Length];

        for (var i = 0; i < Length; i++)
        {
            _nums[i] = random.NextDouble() < ZeroProbability ? 0 : random.Next(1, MaxValueExclusive);
        }

        _nums[0] = 0;
    }

    [Benchmark(Baseline = true)]
    public int BruteForceSimulation() =>
        MakeArrayElementsEqualToZeroSolution.CountValidSelectionsByBruteForceSimulation(_nums);

    [Benchmark]
    public int PrefixSumBalance() =>
        MakeArrayElementsEqualToZeroSolution.CountValidSelectionsByPrefixSumBalance(_nums);
}
