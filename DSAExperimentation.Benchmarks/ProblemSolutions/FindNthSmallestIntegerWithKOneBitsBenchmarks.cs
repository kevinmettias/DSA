using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindNthSmallestIntegerWithKOneBits;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindNthSmallestIntegerWithKOneBitsSolution's,
// the same methods FindNthSmallestIntegerWithKOneBitsTests proves correct. K
// stays small and fixed so Position's growth keeps the answer - and so the
// PopCountScan baseline's walk - within a benchmarkable range; the Params
// values scale the count of one-bit positions the scan must pass over.
[MemoryDiagnoser]
public class FindNthSmallestIntegerWithKOneBitsBenchmarks
{
    private const int K = 4;

    [Params(200, 2000)]
    public long Position { get; set; }

    [Benchmark(Baseline = true)]
    public long PopCountScan() => FindNthSmallestIntegerWithKOneBitsSolution.NthSmallestByPopCountScan(Position, K);

    [Benchmark]
    public long MemoizedBinomialSelection() =>
        FindNthSmallestIntegerWithKOneBitsSolution.NthSmallestByMemoizedBinomialSelection(Position, K);
}
