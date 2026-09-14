using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumNumberOfSwapsToMakeTheStringBalanced;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumNumberOfSwapsToMakeTheStringBalancedSolution's.
// _text is "]" repeated Length/2 times followed by "[" repeated Length/2 times, forcing
// every leading ']' through a full backward scan of the remaining closer run before the
// baseline finds its first available opener, instead of an early exit on the first
// invocation making it look artificially competitive.
[MemoryDiagnoser]
public class MinimumNumberOfSwapsToMakeTheStringBalancedBenchmarks
{
    private const int TextHalfDivisor = 2; // _text is split into two equal-length halves

    [Params(200, 4_000)]
    public int Length;

    private string _text = null!;

    [GlobalSetup]
    public void Setup()
    {
        var half = Length / TextHalfDivisor;
        _text = new string(']', half) + new string('[', half);
    }

    [Benchmark(Baseline = true)]
    public int BackwardScan() =>
        MinimumNumberOfSwapsToMakeTheStringBalancedSolution.MinSwapsByBackwardScan(_text);

    [Benchmark]
    public int StackScan() =>
        MinimumNumberOfSwapsToMakeTheStringBalancedSolution.MinSwapsByStack(_text);
}
