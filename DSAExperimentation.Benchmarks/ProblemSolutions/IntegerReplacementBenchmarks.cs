using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.IntegerReplacement;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are IntegerReplacementSolution's, the same methods
// IntegerReplacementTests proves correct. N is deliberately the repeating-bit
// pattern 0b0101...01 at two bit-lengths (not a "typical" random value): every
// bit position forces an odd branch, so the unmemoized arm's call tree explodes
// into millions of redundant calls (~10.9M for the 31-bit case) while the
// memoized arm only ever computes a few dozen distinct values along the way
// (~90 for the same case) - this recurrence's actual reconvergence-driven
// asymptotic gap, not an artifact of a convenient input.
[MemoryDiagnoser]
public class IntegerReplacementBenchmarks
{
    [Params(21_845, 1_431_655_765)]
    public int N { get; set; }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() => IntegerReplacementSolution.MinStepsByUnmemoizedRecursion(N);

    [Benchmark]
    public int MemoizedRecurrence() => IntegerReplacementSolution.MinStepsByMemoizedRecurrence(N);
}
