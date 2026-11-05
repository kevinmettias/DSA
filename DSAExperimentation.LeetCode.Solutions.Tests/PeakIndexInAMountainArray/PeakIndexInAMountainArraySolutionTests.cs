using DSAExperimentation.LeetCode.Solutions.PeakIndexInAMountainArray;

namespace DSAExperimentation.LeetCode.Solutions.Tests.PeakIndexInAMountainArray;

// Harness only. Both strategies are PeakIndexInAMountainArraySolution's - this
// file pins them to the two ends of the
// mountain (peak at index 1, the earliest LC allows, and peak at the last-but-one
// index). The linear scan was previously a benchmark-only baseline and had never
// been asserted against anything.
public sealed partial class PeakIndexInAMountainArraySolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [0, 1, 2, 3, 4, 5, 6, 7, 8, 1], 8 },
            { [3, 4, 5, 1], 2 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void PeakIndexByPredicateSearch_MountainArrayExamples_ReturnsPeakIndex(
        int[] mountain, int expected) =>
        Assert.Equal(expected, PeakIndexInAMountainArraySolution.PeakIndexByPredicateSearch(mountain));

    [Theory]
    [MemberData(nameof(Cases))]
    public void PeakIndexByLinearScan_MountainArrayExamples_ReturnsPeakIndex(
        int[] mountain, int expected) =>
        Assert.Equal(expected, PeakIndexInAMountainArraySolution.PeakIndexByLinearScan(mountain));
}
