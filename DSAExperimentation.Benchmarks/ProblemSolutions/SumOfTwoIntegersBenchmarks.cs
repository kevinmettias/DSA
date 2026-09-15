using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SumOfTwoIntegers;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SumOfTwoIntegersSolution's, the same methods
// SumOfTwoIntegersTests proves correct - the forbidden-by-the-problem
// BuiltInAdd (plain +) vs. the compliant BitwiseCarryLoop that reaches the
// same result via XOR/AND-shift carry propagation only, the same
// "trivial-but-disallowed operator baseline vs. the actually-compliant
// algorithm" pairing DivideTwoIntegersBenchmarks uses for BuiltInDivide vs.
// BinarySearchProduct.
[MemoryDiagnoser]
public class SumOfTwoIntegersBenchmarks
{
    private const int B = 123_456_789;

    [Params(1_000, 1_000_000)]
    public int A { get; set; }

    [Benchmark(Baseline = true)]
    public int BuiltInAdd() => SumOfTwoIntegersSolution.GetSumByBuiltInAddition(A, B);

    [Benchmark]
    public int BitwiseCarryLoop() => SumOfTwoIntegersSolution.GetSumByBitwiseCarryLoop(A, B);
}
