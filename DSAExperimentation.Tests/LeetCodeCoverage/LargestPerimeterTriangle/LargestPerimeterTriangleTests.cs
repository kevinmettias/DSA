using DSAExperimentation.LeetCode.LargestPerimeterTriangle;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LargestPerimeterTriangle;

// Harness only. Both strategies are LargestPerimeterTriangleSolution's - this file
// just pins them to LeetCode's published examples, the no-valid-triangle case, and a
// case where the largest sides cannot form a triangle but a smaller triple can, so
// the sorted scan has to keep walking down rather than stop at the top.
public sealed class LargestPerimeterTriangleTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [2, 1, 2], 5 },
            { [1, 2, 1, 10], 0 },
            { [1, 2, 1, 10, 6, 5], 21 },
            { [3, 6, 2, 3], 8 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LargestPerimeterByBruteForceTriples_LeetCodeExamples_ReturnsSumOfBestValidTriple(
        int[] nums, int expected) =>
        Assert.Equal(expected, LargestPerimeterTriangleSolution.LargestPerimeterByBruteForceTriples(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LargestPerimeterBySortedScan_LeetCodeExamples_ReturnsSumOfBestValidTriple(
        int[] nums, int expected) =>
        Assert.Equal(expected, LargestPerimeterTriangleSolution.LargestPerimeterBySortedScan(nums));
}
