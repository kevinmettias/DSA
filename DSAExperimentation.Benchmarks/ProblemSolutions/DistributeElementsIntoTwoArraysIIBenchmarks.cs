using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DistributeElementsIntoTwoArraysII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DistributeElementsIntoTwoArraysIISolution's, the
// same methods DistributeElementsIntoTwoArraysIITests proves correct.
[MemoryDiagnoser]
public class DistributeElementsIntoTwoArraysIIBenchmarks
{
    private const int MaxValueExclusive = 1_000_000_000;
    private const int Seed = 3072;

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForce() => DistributeElementsIntoTwoArraysIISolution.DistributeByBruteForce(_nums);

    [Benchmark]
    public int[] FenwickTree() => DistributeElementsIntoTwoArraysIISolution.DistributeByFenwickTree(_nums);
}
