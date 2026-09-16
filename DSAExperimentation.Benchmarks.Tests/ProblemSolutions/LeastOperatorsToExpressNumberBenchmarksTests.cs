using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LeastOperatorsToExpressNumberBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the base-digit recursion un-memoized against the same
// recursion memoized on its (remaining, level) pairs - so a harness whose arms disagree is timing
// two different problems. Setup derives the target from one bit length, so the same TargetBitLength
// must rebuild the same target; otherwise two published numbers were never comparable in the first
// place.
public sealed partial class LeastOperatorsToExpressNumberBenchmarksTests
{
    private const int SmallestTargetBitLength = 16;

    [Fact]
    public void Setup_SameTargetBitLength_RebuildsTheSameTarget() =>
        Assert.Equal(BuildHarness().UnmemoizedRecursion(), BuildHarness().UnmemoizedRecursion());

    [Fact]
    public void UnmemoizedRecursion_AllOnesTargetInBaseTwo_AgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecursion(), harness.UnmemoizedRecursion());
    }

    [Fact]
    public void MemoizedRecursion_AllOnesTargetInBaseTwo_AgreesWithUnmemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.UnmemoizedRecursion(), harness.MemoizedRecursion());
    }

    private static LeastOperatorsToExpressNumberBenchmarks BuildHarness()
    {
        var harness = new LeastOperatorsToExpressNumberBenchmarks { TargetBitLength = SmallestTargetBitLength };
        harness.Setup();

        return harness;
    }
}
