using DSAExperimentation.LeetCode.FindPolygonWithTheLargestPerimeter;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindPolygonWithTheLargestPerimeter;

// Harness only. Both strategies are
// FindPolygonWithTheLargestPerimeterSolution's - this file just pins them to
// LeetCode's published examples, including the case where no valid polygon
// exists at all.
public sealed class FindPolygonWithTheLargestPerimeterTests
{
    public static TheoryData<int[], long> Examples =>
        new()
        {
            { [5, 5, 5], 15 },
            { [1, 12, 1, 2, 5, 50, 3], 12 },
            { [5, 5, 50], -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LargestPerimeterByBruteForceSubsets_LeetCodeExamples_ReturnsLargestValidPerimeter(int[] nums, long expected) =>
        Assert.Equal(expected, FindPolygonWithTheLargestPerimeterSolution.LargestPerimeterByBruteForceSubsets(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LargestPerimeterBySortedRunningSum_LeetCodeExamples_ReturnsLargestValidPerimeter(int[] nums, long expected) =>
        Assert.Equal(expected, FindPolygonWithTheLargestPerimeterSolution.LargestPerimeterBySortedRunningSum(nums));
}
