using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SqrtXBenchmarks (ARCHITECTURE 17.9): the smallest param is a perfect
// square, so the fixture itself names the answer: the floor of its square root is exactly the
// number whose square it is. Asserting it of the baseline is what makes the generic arm check's
// agreement decisive rather than two arms sharing one wrong root.
public sealed partial class SqrtXBenchmarksTests
{
    private const int SmallestValue = 10_000;

    // 10_000 is 100 squared, so its integer square root is exactly 100.
    private const int ExpectedRoot = 100;

    [Fact]
    public void NewtonIteration_TenThousand_ReturnsTheExactRoot() =>
        Assert.Equal(ExpectedRoot, BuildHarness().NewtonIteration());

    private static SqrtXBenchmarks BuildHarness() => new() { Value = SmallestValue };
}
