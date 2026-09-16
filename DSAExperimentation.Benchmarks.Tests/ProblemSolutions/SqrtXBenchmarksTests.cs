using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SqrtXBenchmarks (ARCHITECTURE 17.9): both arms are SqrtXSolution's integer
// square roots of the same Value - the BCL's own double square root against a hand-rolled binary
// search - so a harness whose arms disagree is timing two different problems. There is no
// [GlobalSetup] to rebuild: the whole input is Value, which the caller hands each arm directly.
//
// The smallest param is a perfect square, so the fixture itself names the answer: the floor of its
// square root is exactly the number whose square it is. Asserting that alongside the agreement is
// what makes the comparison decisive rather than two arms sharing one wrong root.
public sealed partial class SqrtXBenchmarksTests
{
    private const int SmallestValue = 10_000;

    // 10_000 is 100 squared, so its integer square root is exactly 100.
    private const int ExpectedRoot = 100;

    [Fact]
    public void BinarySearchRoot_TenThousand_AgreesWithMathSqrt()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedRoot, harness.BinarySearchRoot());
        Assert.Equal(harness.MathSqrt(), harness.BinarySearchRoot());
    }

    [Fact]
    public void MathSqrt_TenThousand_AgreesWithBinarySearchRoot()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedRoot, harness.MathSqrt());
        Assert.Equal(harness.BinarySearchRoot(), harness.MathSqrt());
    }

    private static SqrtXBenchmarks BuildHarness() => new() { Value = SmallestValue };
}
