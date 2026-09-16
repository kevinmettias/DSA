using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MedianOfTwoSortedArraysBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - merging both arrays and reading off the middle against
// partitioning the two sorted arrays so the halves balance - so a harness whose arms disagree is timing
// two different problems. Both arms return the median as a double, and a double is compared under a
// named tolerance rather than by exact equality. Both arms only read the two arrays, so one harness is
// safe to read twice in either order. Setup draws both arrays from one fixed seed and sorts them, so
// the same TotalLength must rebuild the same pair of sorted arrays and with it the same median.
public sealed partial class MedianOfTwoSortedArraysBenchmarksTests
{
    private const int SmallestTotalLength = 2_000;

    private const double RelativeTolerance = 1e-9;

    [Fact]
    public void Setup_SameTotalLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().MergeAndSort(),
            BuildHarness().MergeAndSort(),
            RelativeTolerance);

    [Fact]
    public void MergeAndSort_SeededSortedArrays_AgreesWithBinarySearchPartition()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BinarySearchPartition(), harness.MergeAndSort(), RelativeTolerance);
    }

    [Fact]
    public void BinarySearchPartition_SeededSortedArrays_AgreesWithMergeAndSort()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MergeAndSort(), harness.BinarySearchPartition(), RelativeTolerance);
    }

    private static MedianOfTwoSortedArraysBenchmarks BuildHarness()
    {
        var harness = new MedianOfTwoSortedArraysBenchmarks { TotalLength = SmallestTotalLength };
        harness.Setup();

        return harness;
    }
}
