using DSAExperimentation.LeetCode.SumOfTwoIntegers;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SumOfTwoIntegersSolution's, the same methods
// SumOfTwoIntegersSolutionTests proves correct, and neither uses the + or - LC 371
// forbids - the ripple-carry adder that walks all 32 bit positions one at a time vs.
// the carry loop that propagates every position's carry at once, XOR/AND-shift, until
// none is left.
//
// Both addends stay inside LC 371's [-1000, 1000]. The second is its most negative
// value, so at the larger first addend the two cancel exactly and the carry ripples up
// through the rest of the word, where the smaller one stops after a single pass. The
// ripple-carry adder takes 32 steps either way.
public class SumOfTwoIntegersBenchmarks
{
    private const int SecondAddend = -1_000;

    [Params(100, 1_000)]
    public int FirstAddend { get; set; }

    [Benchmark(Baseline = true)]
    public int RippleCarryAdder() => SumOfTwoIntegersSolution.GetSumByRippleCarryAdder(FirstAddend, SecondAddend);

    [Benchmark]
    public int BitwiseCarryLoop() => SumOfTwoIntegersSolution.GetSumByBitwiseCarryLoop(FirstAddend, SecondAddend);
}
