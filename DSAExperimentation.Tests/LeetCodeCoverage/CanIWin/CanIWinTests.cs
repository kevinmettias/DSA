using DSAExperimentation.LeetCode.CanIWin;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CanIWin;

// Harness only. Both strategies are CanIWinSolution's - LeetCode's published
// examples are stated once and replayed against each, so a failure names the
// strategy that broke rather than reporting a disagreement between an anonymous
// test helper and an anonymous benchmark arm.
public sealed class CanIWinTests
{
    public static TheoryData<int, int, bool> Examples =>
        new()
        {
            { 10, 11, false },
            { 10, 0, true },
            { 10, 1, true },
            { 10, 40, false },
            { 4, 11, false },
            { 4, 6, true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanWinByBruteForceRecursion_LeetCodeExamples_MatchesExpectedOutcome(
        int maxChoosableInteger, int desiredTotal, bool expected) =>
        Assert.Equal(expected, CanIWinSolution.CanWinByBruteForceRecursion(maxChoosableInteger, desiredTotal));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanWinByMemoizedRecursion_LeetCodeExamples_MatchesExpectedOutcome(
        int maxChoosableInteger, int desiredTotal, bool expected) =>
        Assert.Equal(expected, CanIWinSolution.CanWinByMemoizedRecursion(maxChoosableInteger, desiredTotal));
}
