using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfWaysToReachAPositionAfterExactlyKStepsBenchmarks (ARCHITECTURE 17.9):
// its two arms are competing strategies for the same question - the un-memoized 2^StepCount walk
// against the memoized one - so a harness whose arms disagree is timing two different problems.
// The class has no [GlobalSetup], so a harness is a bare initializer plus the tuned StepCount; the
// smallest tuned value is used because it is the one the un-memoized arm can still afford.
public sealed partial class NumberOfWaysToReachAPositionAfterExactlyKStepsBenchmarksTests
{
    private const int SmallestStepCount = 18;

    [Fact]
    public void MemoizedRecursion_SmallestStepCount_AgreesWithUnmemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.UnmemoizedRecursion(), harness.MemoizedRecursion());
    }

    [Fact]
    public void UnmemoizedRecursion_SmallestStepCount_AgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecursion(), harness.UnmemoizedRecursion());
    }

    private static NumberOfWaysToReachAPositionAfterExactlyKStepsBenchmarks BuildHarness() =>
        new() { StepCount = SmallestStepCount };
}
