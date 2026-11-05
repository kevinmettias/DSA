using DSAExperimentation.LeetCode.Solutions.LargestPerimeterTriangle;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LargestPerimeterTriangle;

// Harness only. Both strategies are LargestPerimeterTriangleSolution's - this file
// pins them to one unsorted input whose three largest sides form the best triangle.
// The no-valid-triangle case and the case where the sorted scan has to keep walking
// down past the largest sides are catalog cases. LeetCode's published examples run
// in CatalogCaseTests.
public sealed partial class LargestPerimeterTriangleSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [1, 2, 1, 10, 6, 5], 21 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void LargestPerimeterByBruteForceTriples_Cases_ReturnsSumOfBestValidTriple(
        int[] nums, int expected) =>
        Assert.Equal(expected, LargestPerimeterTriangleSolution.LargestPerimeterByBruteForceTriples(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void LargestPerimeterBySortedScan_Cases_ReturnsSumOfBestValidTriple(
        int[] nums, int expected) =>
        Assert.Equal(expected, LargestPerimeterTriangleSolution.LargestPerimeterBySortedScan(nums));
}
