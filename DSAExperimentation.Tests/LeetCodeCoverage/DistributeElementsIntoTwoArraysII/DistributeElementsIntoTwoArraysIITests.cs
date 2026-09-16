using DSAExperimentation.LeetCode.DistributeElementsIntoTwoArraysII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DistributeElementsIntoTwoArraysII;

// Harness only. Both strategies are DistributeElementsIntoTwoArraysIISolution's
// - this file just pins them to LeetCode's published examples.
public sealed partial class DistributeElementsIntoTwoArraysIITests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [2, 1, 3, 3], [2, 3, 1, 3] },
            { [5, 14, 3, 1, 2], [5, 3, 1, 2, 14] },
            { [3, 3, 3, 3], [3, 3, 3, 3] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void DistributeByBruteForce_LeetCodeExamples_ReturnsResultArray(int[] nums, int[] expected) =>
        Assert.Equal(expected, DistributeElementsIntoTwoArraysIISolution.DistributeByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void DistributeByFenwickTree_LeetCodeExamples_ReturnsResultArray(int[] nums, int[] expected) =>
        Assert.Equal(expected, DistributeElementsIntoTwoArraysIISolution.DistributeByFenwickTree(nums));
}
