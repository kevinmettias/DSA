using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ReverseBits;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ReverseBitsSolution's, the same methods
// ReverseBitsTests proves correct. Pre-migration this class was an untested
// compile-smoke placeholder (Baseline() => 1, PrimitiveComposed() => 1) rather
// than a second strategy to reconcile. The operand sets every bit but one, so
// every shift/mask step runs and each of the four lookup bytes is dense.
[MemoryDiagnoser]
public class ReverseBitsBenchmarks
{
    // LeetCode's own second example: every bit but one set, exercising every
    // shift/mask step rather than a sparse pattern that could special-case.
    private const uint N = 4294967293u;

    [Benchmark(Baseline = true)]
    public uint BitShift() => ReverseBitsSolution.ReverseByBitShift(N);

    [Benchmark]
    public uint ByteLookup() => ReverseBitsSolution.ReverseByByteLookup(N);
}
