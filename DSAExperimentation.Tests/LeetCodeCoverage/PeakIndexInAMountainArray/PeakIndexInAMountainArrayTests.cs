using DSAExperimentation.LeetCode.PeakIndexInAMountainArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PeakIndexInAMountainArray;

// Harness only. Both strategies are PeakIndexInAMountainArraySolution's - this
// file pins them to LeetCode's published examples plus the two ends of the
// mountain (peak at index 1, the earliest LC allows, and peak at the last-but-one
// index). The linear scan was previously a benchmark-only baseline and had never
// been asserted against anything.
public sealed partial class PeakIndexInAMountainArrayTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [0, 1, 0], 1 },
            { [0, 2, 1, 0], 1 },
            { [0, 10, 5, 2], 1 },
            { [24, 69, 100, 99, 79, 78, 67, 36, 26, 19], 2 },
            { [0, 1, 2, 3, 4, 5, 6, 7, 8, 1], 8 },
            { [3, 4, 5, 1], 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void PeakIndexByBinarySearchLowerBound_MountainArrayExamples_ReturnsPeakIndex(
        int[] mountain, int expected) =>
        Assert.Equal(expected, PeakIndexInAMountainArraySolution.PeakIndexByBinarySearchLowerBound(mountain));

    [Theory]
    [MemberData(nameof(Examples))]
    public void PeakIndexByLinearScan_MountainArrayExamples_ReturnsPeakIndex(
        int[] mountain, int expected) =>
        Assert.Equal(expected, PeakIndexInAMountainArraySolution.PeakIndexByLinearScan(mountain));
}
