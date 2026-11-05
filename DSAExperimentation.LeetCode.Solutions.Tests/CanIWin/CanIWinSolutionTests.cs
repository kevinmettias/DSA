using DSAExperimentation.LeetCode.Solutions.CanIWin;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CanIWin;

// Harness only. Both strategies are CanIWinSolution's - the cases are stated once
// and replayed against each, so a failure names the strategy that broke rather than
// reporting a disagreement between an anonymous test helper and an anonymous
// benchmark arm. They are a total the whole pool cannot reach (1 through 4 sum to
// 10, short of 11), which the first player loses, and a small game the first player
// wins. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class CanIWinSolutionTests
{
    public static TheoryData<CanWinCase> Cases =>
        new()
        {
            { new CanWinCase(MaxChoosableInteger: 4, DesiredTotal: 11, Expected: false) },
            { new CanWinCase(MaxChoosableInteger: 4, DesiredTotal: 6, Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanWinByBruteForceRecursion_Cases_MatchesExpectedOutcome(CanWinCase testCase)
    {
        var canWin = CanIWinSolution.CanWinByBruteForceRecursion(
            testCase.MaxChoosableInteger, testCase.DesiredTotal);

        Assert.Equal(testCase.Expected, canWin);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanWinByMemoizedRecursion_Cases_MatchesExpectedOutcome(CanWinCase testCase)
    {
        var canWin = CanIWinSolution.CanWinByMemoizedRecursion(testCase.MaxChoosableInteger, testCase.DesiredTotal);

        Assert.Equal(testCase.Expected, canWin);
    }

    // One case: the largest choosable integer, the total that decides the
    // game, and whether the first player wins. The row names every position - a bare
    // `bool` argument would read as "true" and say nothing about what is true.
    public readonly record struct CanWinCase(int MaxChoosableInteger, int DesiredTotal, bool Expected);
}
