using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SubarraysDistinctElementSumOfSquaresI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SubarraysDistinctElementSumOfSquaresISolution's, the
// same methods SubarraysDistinctElementSumOfSquaresITests proves correct. The
// baseline re-derives each subarray's distinct count with a linear scan (O(n^3));
// the composed arm grows one Set<int> per start index (O(n^2)). Params stay at or
// under 100, this problem's own constraint on nums.Length.
[MemoryDiagnoser]
public class SubarraysDistinctElementSumOfSquaresIBenchmarks
{
    private const int RandomSeed = 1;
    private const int MaxValueExclusive = 101;

    private int[] _nums = [];

    [Params(20, 100)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long BruteForce() => SubarraysDistinctElementSumOfSquaresISolution.SumOfSquaresByBruteForce(_nums);

    [Benchmark]
    public long GrowingSet() => SubarraysDistinctElementSumOfSquaresISolution.SumOfSquaresByGrowingSet(_nums);
}
