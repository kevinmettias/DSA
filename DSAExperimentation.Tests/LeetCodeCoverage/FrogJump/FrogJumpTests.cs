using DSAExperimentation.LeetCode.FrogJump;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FrogJump;

// Harness only. Both strategies are FrogJumpSolution's - this file just pins
// them to LeetCode's published examples.
public sealed class FrogJumpTests
{
    public static TheoryData<int[], bool> Examples =>
        new()
        {
            { [0, 1, 3, 5, 6, 8, 12, 17], true },
            { [0, 1, 2, 3, 4, 8, 9, 11], false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanCrossByRecursiveBruteForce_LeetCodeExamples_ReturnsWhetherLastStoneIsReachable(
        int[] stones, bool expected) =>
        Assert.Equal(expected, FrogJumpSolution.CanCrossByRecursiveBruteForce(stones));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanCrossByHashMapDynamicProgramming_LeetCodeExamples_ReturnsWhetherLastStoneIsReachable(
        int[] stones, bool expected) =>
        Assert.Equal(expected, FrogJumpSolution.CanCrossByHashMapDynamicProgramming(stones));
}
