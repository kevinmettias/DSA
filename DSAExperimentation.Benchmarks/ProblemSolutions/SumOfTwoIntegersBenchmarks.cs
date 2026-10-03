using DSAExperimentation.LeetCode.SumOfTwoIntegers;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SumOfTwoIntegersSolution's, the same methods
// SumOfTwoIntegersSolutionTests proves correct - the forbidden-by-the-problem
// BuiltInAdd (plain +) vs. the compliant BitwiseCarryLoop that reaches the
// same result via XOR/AND-shift carry propagation only, the same
// "trivial-but-disallowed operator baseline vs. the actually-compliant
// algorithm" pairing DivideTwoIntegersBenchmarks uses for BuiltInDivide vs.
// BinarySearchProduct.
//
// Both addends stay inside LC 371's [-1000, 1000]. The second is its most negative
// value, so at the larger first addend the two cancel exactly and the carry ripples up
// through the rest of the word, where the smaller one stops after a single pass.
public class SumOfTwoIntegersBenchmarks
{
    private const int SecondAddend = -1_000;

    [Params(100, 1_000)]
    public int FirstAddend { get; set; }

    [Benchmark(Baseline = true)]
    public int BuiltInAdd() => SumOfTwoIntegersSolution.GetSumByBuiltInAddition(FirstAddend, SecondAddend);

    [Benchmark]
    public int BitwiseCarryLoop() => SumOfTwoIntegersSolution.GetSumByBitwiseCarryLoop(FirstAddend, SecondAddend);
}
