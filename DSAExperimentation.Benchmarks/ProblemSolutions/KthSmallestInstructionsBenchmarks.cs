using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.KthSmallestInstructions;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are KthSmallestInstructionsSolution's, the same methods
// KthSmallestInstructionsTests proves correct. The destination and the median rank
// are prepared in [GlobalSetup] so the factorial-sized rank arithmetic is not
// charged to either measured method - what is measured is enumerating C(2n, n)
// routes against deciding 2n characters from cached binomial counts.
[MemoryDiagnoser]
public class KthSmallestInstructionsBenchmarks
{
    [Params(5, 8)]
    public int Size;

    private int[] _destination = null!;
    private long _k;

    [GlobalSetup]
    public void Setup()
    {
        _destination = KthSmallestInstructionsWorkloads.SquareDestination(Size);
        _k = KthSmallestInstructionsWorkloads.MedianRank(Size);
    }

    [Benchmark(Baseline = true)]
    public string EnumerateAndSort() =>
        KthSmallestInstructionsSolution.KthSmallestPathByEnumerateAndSort(_destination, _k);

    [Benchmark]
    public string MemoizedGreedy() =>
        KthSmallestInstructionsSolution.KthSmallestPathByMemoizedGreedy(_destination, _k);
}
