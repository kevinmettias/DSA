using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LexicographicallyMaximumMEXArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LexicographicallyMaximumMEXArraySolution's, the
// same methods LexicographicallyMaximumMEXArrayTests proves correct. The brute
// force arm rescans a whole window's MEX from scratch on every element it
// grows into, so Length stays small enough for it to finish in reasonable time;
// the frequency/pointer arm's whole point is that it never rescans.
[MemoryDiagnoser]
public class LexicographicallyMaximumMEXArrayBenchmarks
{
    private const int Seed = 3948; private int[] _nums = [];

    // LC problem number

    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = [.. Enumerable.Range(0, Length).Select(_ => random.Next(0, Length))];
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForce() => LexicographicallyMaximumMEXArraySolution.MexArrayByBruteForce(_nums);

    [Benchmark]
    public int[] FrequencyPointer() => LexicographicallyMaximumMEXArraySolution.MexArrayByFrequencyPointer(_nums);
}
