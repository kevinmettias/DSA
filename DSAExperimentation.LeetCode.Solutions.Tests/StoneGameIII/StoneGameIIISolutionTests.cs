using DSAExperimentation.LeetCode.Solutions.StoneGameIII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.StoneGameIII;

// Harness only. Both strategies are StoneGameIIISolution's -
// WinnerByUnmemoizedRecursion (previously untested scaffolding inlined in the
// benchmark as its baseline arm, and reporting the raw score difference rather than
// LC 1406's winner) now gets the same cases as WinnerByMemoizedRecursion
// (previously the test's own private helper), so a failure names the strategy that
// broke.
public sealed partial class StoneGameIIISolutionTests
{
    public static TheoryData<int[], string> Cases =>
        new()
        {
            { [5], "Alice" },
            { [2, 2], "Alice" },
            { [-1, -2, -3], "Tie" },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void WinnerByUnmemoizedRecursion_Cases_ReturnsWinnerUnderOptimalPlay(
        int[] stoneValue, string expected) =>
        Assert.Equal(expected, StoneGameIIISolution.WinnerByUnmemoizedRecursion(stoneValue));

    [Theory]
    [MemberData(nameof(Cases))]
    public void WinnerByMemoizedRecursion_Cases_ReturnsWinnerUnderOptimalPlay(
        int[] stoneValue, string expected) =>
        Assert.Equal(expected, StoneGameIIISolution.WinnerByMemoizedRecursion(stoneValue));
}
