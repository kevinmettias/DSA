using DSAExperimentation.LeetCode.HouseRobberV;

namespace DSAExperimentation.Tests.LeetCodeCoverage.HouseRobberV;

// Harness only. Both strategies are HouseRobberVSolution's - this file just pins
// them to LeetCode's published examples, including the case where a same-color
// run forces a skip even though the skipped house is worth more than its
// same-color neighbor (example 3: house 1's value 1 loses to robbing houses 0
// and 2 instead).
public sealed class HouseRobberVTests
{
    public static TheoryData<int[], int[], long> Examples =>
        new()
        {
            { [1, 4, 3, 5], [1, 1, 2, 2], 9 },
            { [3, 1, 2, 4], [2, 3, 2, 2], 8 },
            { [10, 1, 3, 9], [1, 1, 1, 2], 22 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxAmountByTabulation_LeetCodeExamples_ReturnsBestNonConflictingSum(
        int[] nums, int[] colors, long expected) =>
        Assert.Equal(expected, HouseRobberVSolution.MaxAmountByTabulation(nums, colors));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxAmountByMemoization_LeetCodeExamples_ReturnsBestNonConflictingSum(
        int[] nums, int[] colors, long expected) =>
        Assert.Equal(expected, HouseRobberVSolution.MaxAmountByMemoization(nums, colors));
}
