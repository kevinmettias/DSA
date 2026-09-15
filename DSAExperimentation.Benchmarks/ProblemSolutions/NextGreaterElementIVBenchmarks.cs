using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NextGreaterElementIV;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NextGreaterElementIVSolution's, the same methods
// NextGreaterElementIVTests proves correct. [GlobalSetup] generates the value array
// - LeetCode's own input shape, handed straight to each strategy, so no
// prepared-input overload is needed - leaving each arm to measure only the sweep.
//
// The O(n^2) brute-force scan walks forward from every index until it has seen two
// greater values; the O(n) sweep pushes each index onto one of two monotonic
// Stack<int>s and lets later values resolve them.
[MemoryDiagnoser]
public class NextGreaterElementIVBenchmarks
{
    private const int RandomSeed = 2454; // LeetCode problem number

    private const int MaxElementValue = 1_000;

    private int[] _values = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxElementValue)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForce() => NextGreaterElementIVSolution.SecondGreaterElementByBruteForce(_values);

    [Benchmark]
    public int[] TwoMonotonicStacks() =>
        NextGreaterElementIVSolution.SecondGreaterElementByTwoMonotonicStacks(_values);
}
