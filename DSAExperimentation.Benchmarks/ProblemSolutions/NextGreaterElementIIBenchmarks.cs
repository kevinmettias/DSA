using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NextGreaterElementII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NextGreaterElementIISolution's, the same methods
// NextGreaterElementIITests proves correct - the O(n^2) scan-ahead baseline vs.
// the O(n) monotonic-stack sweep over this repo's own Stack<int>.
[MemoryDiagnoser]
public class NextGreaterElementIIBenchmarks
{
    private const int RandomSeed = 3;
    private const int MaxElementValue = 1_000;

    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxElementValue)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForce() => NextGreaterElementIISolution.NextGreaterElementsByBruteForce(_values);

    [Benchmark]
    public int[] MonotonicStack() => NextGreaterElementIISolution.NextGreaterElementsByMonotonicStack(_values);
}
