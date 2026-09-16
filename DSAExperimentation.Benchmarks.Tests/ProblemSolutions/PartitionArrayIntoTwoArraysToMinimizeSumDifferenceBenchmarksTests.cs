using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PartitionArrayIntoTwoArraysToMinimizeSumDifferenceBenchmarks
// (ARCHITECTURE 17.9): its two arms are
// PartitionArrayIntoTwoArraysToMinimizeSumDifferenceSolution's, competing searches for the same
// minimum split difference - an exhaustive bitmask scan over the whole array against
// meet-in-the-middle over each half's subset sums grouped by size - so a harness whose arms
// disagree is timing two different problems. Setup draws nums from one fixed seed, so the same
// Length must rebuild the same array.
public sealed partial class PartitionArrayIntoTwoArraysToMinimizeSumDifferenceBenchmarksTests
{
    private const int SmallestLength = 16;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForceAllEqualSplits(), BuildHarness().BruteForceAllEqualSplits());

    [Fact]
    public void BruteForceAllEqualSplits_SeededSignedValues_AgreesWithMeetInTheMiddleGroupedBySize()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.MeetInTheMiddleGroupedBySize(),
            harness.BruteForceAllEqualSplits());
    }

    [Fact]
    public void MeetInTheMiddleGroupedBySize_SeededSignedValues_AgreesWithBruteForceAllEqualSplits()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.BruteForceAllEqualSplits(),
            harness.MeetInTheMiddleGroupedBySize());
    }

    private static PartitionArrayIntoTwoArraysToMinimizeSumDifferenceBenchmarks BuildHarness()
    {
        var harness = new PartitionArrayIntoTwoArraysToMinimizeSumDifferenceBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
