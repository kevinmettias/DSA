using DSAExperimentation.LeetCode.JumpGameIX;

namespace DSAExperimentation.Tests.LeetCodeCoverage.JumpGameIX;

// Harness only. Both reachability strategies are JumpGameIXSolution's - this file
// just pins them to LeetCode's published examples.
public sealed partial class JumpGameIXTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [2, 1, 3], [2, 2, 3] },
            { [2, 3, 1], [3, 3, 3] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxValuesByJumpBfs_LeetCodeExamples_ReturnsMaxReachableValuePerIndex(
        int[] nums, int[] expected) =>
        Assert.Equal(expected, JumpGameIXSolution.MaxValuesByJumpBfs(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxValuesByAdjacentUnionFind_LeetCodeExamples_ReturnsMaxReachableValuePerIndex(
        int[] nums, int[] expected) =>
        Assert.Equal(expected, JumpGameIXSolution.MaxValuesByAdjacentUnionFind(nums));
}
