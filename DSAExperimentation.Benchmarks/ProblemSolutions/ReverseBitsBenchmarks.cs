using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ReverseBits;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is ReverseBitsSolution's, the same method
// ReverseBitsTests proves correct. Pre-migration this class was an untested
// compile-smoke placeholder (Baseline() => 1, PrimitiveComposed() => 1) rather
// than a second strategy to reconcile.
[MemoryDiagnoser]
public class ReverseBitsBenchmarks
{
    // LeetCode's own second example: every bit but one set, exercising every
    // shift/mask step rather than a sparse pattern that could special-case.
    private const uint N = 4294967293u;

    [Benchmark(Baseline = true)]
    public uint BitShift() => ReverseBitsSolution.ReverseByBitShift(N);
}
