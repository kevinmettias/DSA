using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RemoveBoxesBenchmarks (ARCHITECTURE 17.9): both arms are RemoveBoxesSolution's,
// competing strategies for the same question - the plain recurrence against the same recurrence
// behind this repo's Memoizer - so a harness whose arms disagree scores two different removal orders.
// Setup builds the boxes from one seeded fixture, so the same BoxCount must rebuild the same boxes.
public sealed partial class RemoveBoxesBenchmarksTests
{
    private const int SmallestBoxCount = 16;

    [Fact]
    public void Setup_SameBoxCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().UnmemoizedRecursion(),
            BuildHarness().UnmemoizedRecursion());

    [Fact]
    public void UnmemoizedRecursion_AgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecursion(), harness.UnmemoizedRecursion());
    }

    [Fact]
    public void MemoizedRecursion_AgreesWithUnmemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.UnmemoizedRecursion(), harness.MemoizedRecursion());
    }

    private static RemoveBoxesBenchmarks BuildHarness()
    {
        var harness = new RemoveBoxesBenchmarks { BoxCount = SmallestBoxCount };
        harness.Setup();

        return harness;
    }
}
