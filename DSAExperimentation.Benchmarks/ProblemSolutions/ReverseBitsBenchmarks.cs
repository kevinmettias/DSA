using DSAExperimentation.LeetCode.ReverseBits;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ReverseBitsSolution's, the same methods
// ReverseBitsSolutionTests proves correct. Pre-migration this class was an untested
// compile-smoke placeholder (Baseline() => 1, PrimitiveComposed() => 1) rather
// than a second strategy to reconcile. The operand sets bits 2 through 30, so
// every shift/mask step runs and each of the four lookup bytes is dense. No
// [Params] axis: the input is one 32-bit word, so there is no size to scale.
public class ReverseBitsBenchmarks
{
    // LeetCode's own second example: bits 2 through 30 set - one bit short of the
    // densest operand LC 190's even n <= 2^31 - 2 allows - exercising every
    // shift/mask step rather than a sparse pattern that could special-case.
    private const uint N = 2147483644u;

    [Benchmark(Baseline = true)]
    public uint BitShift() => ReverseBitsSolution.ReverseByBitShift(N);

    [Benchmark]
    public uint ByteLookup() => ReverseBitsSolution.ReverseByByteLookup(N);
}
