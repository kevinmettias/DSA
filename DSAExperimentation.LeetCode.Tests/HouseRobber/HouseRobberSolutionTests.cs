using DSAExperimentation.LeetCode.HouseRobber;

namespace DSAExperimentation.LeetCode.Tests.HouseRobber;

// Harness only: both strategies live in HouseRobberSolution and are asserted
// against LeetCode's published examples, plus a couple of thin edge cases the
// original test never covered.
public sealed partial class HouseRobberSolutionTests
{
    // The street the stretch tests rob parts of.
    private static readonly int[] Street = [2, 7, 9, 3, 1];

    public static TheoryData<int[], int> Examples =>
        new()
        {
            { new[] { 1, 2, 3, 1 }, 4 },
            { new[] { 2, 7, 9, 3, 1 }, 12 },
            { new[] { 5 }, 5 },
            { new[] { 2, 1, 1, 2 }, 4 },
        };

    // Stretches of one street, [2, 7, 9, 3, 1], as [start, end) house indices: houses outside the
    // stretch are never robbed, whatever they would add, and an empty stretch hauls nothing.
    public static TheoryData<int, int, int> Stretches =>
        new()
        {
            { 3, 5, 3 },
            { 0, 4, 11 },
            { 1, 4, 10 },
            { 2, 2, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RobByMemoizedRecursion_LeetCodeExamples_ReturnsBestNonAdjacentSum(int[] nums, int expected) =>
        Assert.Equal(expected, HouseRobberSolution.RobByMemoizedRecursion(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void RobByIterativeRollingTotals_LeetCodeExamples_ReturnsBestNonAdjacentSum(int[] nums, int expected) =>
        Assert.Equal(expected, HouseRobberSolution.RobByIterativeRollingTotals(nums));

    [Theory]
    [MemberData(nameof(Stretches))]
    public void RobByMemoizedRecursion_HouseRange_RobsOnlyThatStretch(int start, int end, int expected) =>
        Assert.Equal(expected, HouseRobberSolution.RobByMemoizedRecursion(Street, start..end));

    [Theory]
    [MemberData(nameof(Stretches))]
    public void RobByIterativeRollingTotals_HouseRange_RobsOnlyThatStretch(int start, int end, int expected) =>
        Assert.Equal(expected, HouseRobberSolution.RobByIterativeRollingTotals(Street, start..end));
}
