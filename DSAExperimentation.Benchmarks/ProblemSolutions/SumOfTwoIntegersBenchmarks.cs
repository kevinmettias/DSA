using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Sum of Two Integers (LC 371): the forbidden-by-the-problem BuiltInAdd (plain +)
// vs. the compliant BitwiseCarryLoop that reaches the same result via XOR/AND-shift
// carry propagation only - the same "trivial-but-disallowed operator baseline vs.
// the actually-compliant algorithm" pairing DivideTwoIntegersBenchmarks already
// uses for BuiltInDivide vs. BinarySearchProduct. No repo primitive applies to
// either side, matching MaximumSubarrayBenchmarks' precedent for a pure scalar
// bit-manipulation problem.
[MemoryDiagnoser]
public class SumOfTwoIntegersBenchmarks
{
    [Params(1_000, 1_000_000)]
    public int A;

    private const int B = 123_456_789;

    [Benchmark(Baseline = true)]
    public int BuiltInAdd() => A + B;

    [Benchmark]
    public int BitwiseCarryLoop()
    {
        var a = A;
        var b = B;

        while (b != 0)
        {
            var carry = (a & b) << 1;
            a ^= b;
            b = carry;
        }

        return a;
    }
}
