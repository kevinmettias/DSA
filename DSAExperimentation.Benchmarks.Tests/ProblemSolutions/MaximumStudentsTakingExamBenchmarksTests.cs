using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumStudentsTakingExamBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the unmemoized row-by-row recursion against the
// bitmask memoization over the same SeatMasks - so a harness whose arms disagree is timing two
// different problems. Setup builds the SeatMasks from the seeded row count, so the same RowCount must
// rebuild the same workload; otherwise two published numbers were never comparable in the first
// place.
public sealed partial class MaximumStudentsTakingExamBenchmarksTests
{
    private const int SmallestRowCount = 3;

    [Fact]
    public void Setup_SameRowCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForceRecursion(), BuildHarness().BruteForceRecursion());

    [Fact]
    public void BruteForceRecursion_AllSeatsOpen_AgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecursion(), harness.BruteForceRecursion());
    }

    [Fact]
    public void MemoizedRecursion_AllSeatsOpen_AgreesWithBruteForceRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceRecursion(), harness.MemoizedRecursion());
    }

    private static MaximumStudentsTakingExamBenchmarks BuildHarness()
    {
        var harness = new MaximumStudentsTakingExamBenchmarks { RowCount = SmallestRowCount };
        harness.Setup();

        return harness;
    }
}
