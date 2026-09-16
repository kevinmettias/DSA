using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfIncreasingPathsInAGridBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - re-walking every shared sub-path from scratch against
// this repo's own Memoizer collapsing those sub-paths within one start's search - so a harness whose
// arms disagree is counting the paths of two different matrices. Setup builds the row-major
// strictly-increasing Size x Size matrix, so the same Size must rebuild the same cells.
//
// Both arms return an int, so they are compared directly. The matrix is read-only for both arms and
// the memoized arm's cache is per start cell, so one harness serves both arms in either order.
public sealed partial class NumberOfIncreasingPathsInAGridBenchmarksTests
{
    private const int SmallestSize = 6;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().NaiveRecursion(), BuildHarness().NaiveRecursion());

    [Fact]
    public void NaiveRecursion_AgreesWithMemoizedRecurrence()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecurrence(), harness.NaiveRecursion());
    }

    [Fact]
    public void MemoizedRecurrence_AgreesWithNaiveRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.NaiveRecursion(), harness.MemoizedRecurrence());
    }

    private static NumberOfIncreasingPathsInAGridBenchmarks BuildHarness()
    {
        var harness = new NumberOfIncreasingPathsInAGridBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
