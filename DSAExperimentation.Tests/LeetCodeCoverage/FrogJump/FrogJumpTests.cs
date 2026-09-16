using DSAExperimentation.LeetCode.FrogJump;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FrogJump;

// Harness only. Both strategies are FrogJumpSolution's - this file just pins
// them to LeetCode's published examples.
public sealed class FrogJumpTests
{
    public static TheoryData<FrogJumpCase> Examples =>
        new()
        {
            { new FrogJumpCase([0, 1, 3, 5, 6, 8, 12, 17], CanCross: true) },
            { new FrogJumpCase([0, 1, 2, 3, 4, 8, 9, 11], CanCross: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanCrossByRecursiveBruteForce_LeetCodeExamples_ReturnsWhetherLastStoneIsReachable(
        FrogJumpCase example) =>
        Assert.Equal(example.CanCross, FrogJumpSolution.CanCrossByRecursiveBruteForce(example.Stones));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanCrossByHashMapDynamicProgramming_LeetCodeExamples_ReturnsWhetherLastStoneIsReachable(
        FrogJumpCase example) =>
        Assert.Equal(example.CanCross, FrogJumpSolution.CanCrossByHashMapDynamicProgramming(example.Stones));

    // One LeetCode example: the stone positions and whether the last stone can be
    // reached. The expected value is named at every construction site, so a row reads as
    // the case it is rather than as a bare `true` whose meaning is its position. Nested
    // because it is only ever used inside this test class - it is this harness's own
    // vocabulary, not a type another file would import.
    public readonly record struct FrogJumpCase(int[] Stones, bool CanCross);
}
