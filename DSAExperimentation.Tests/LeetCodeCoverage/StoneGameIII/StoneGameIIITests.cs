using DSAExperimentation.LeetCode.StoneGameIII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StoneGameIII;

// Harness only. Both strategies are StoneGameIIISolution's -
// WinnerByUnmemoizedRecursion (previously untested scaffolding inlined in the
// benchmark as its baseline arm, and reporting the raw score difference rather than
// LC 1406's winner) now gets the same examples as WinnerByMemoizedRecursion
// (previously the test's own private helper), so a failure names the strategy that
// broke.
public sealed partial class StoneGameIIITests
{
    public static TheoryData<int[], string> Examples =>
        new()
        {
            { [1, 2, 3, 7], "Bob" },
            { [1, 2, 3, -9], "Alice" },
            { [1, 2, 3, 6], "Tie" },
            { [5], "Alice" },
            { [2, 2], "Alice" },
            { [-1, -2, -3], "Tie" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void WinnerByUnmemoizedRecursion_LeetCodeExamples_ReturnsWinnerUnderOptimalPlay(
        int[] stoneValue, string expected) =>
        Assert.Equal(expected, StoneGameIIISolution.WinnerByUnmemoizedRecursion(stoneValue));

    [Theory]
    [MemberData(nameof(Examples))]
    public void WinnerByMemoizedRecursion_LeetCodeExamples_ReturnsWinnerUnderOptimalPlay(
        int[] stoneValue, string expected) =>
        Assert.Equal(expected, StoneGameIIISolution.WinnerByMemoizedRecursion(stoneValue));
}
