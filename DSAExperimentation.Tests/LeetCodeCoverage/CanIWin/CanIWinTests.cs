using DSAExperimentation.LeetCode.CanIWin;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CanIWin;

// Harness only. Both strategies are CanIWinSolution's - LeetCode's published
// examples are stated once and replayed against each, so a failure names the
// strategy that broke rather than reporting a disagreement between an anonymous
// test helper and an anonymous benchmark arm.
public sealed partial class CanIWinTests
{
    public static TheoryData<CanWinExample> Examples =>
        new()
        {
            { new CanWinExample(MaxChoosableInteger: 10, DesiredTotal: 11, Expected: false) },
            { new CanWinExample(MaxChoosableInteger: 10, DesiredTotal: 0, Expected: true) },
            { new CanWinExample(MaxChoosableInteger: 10, DesiredTotal: 1, Expected: true) },
            { new CanWinExample(MaxChoosableInteger: 10, DesiredTotal: 40, Expected: false) },
            { new CanWinExample(MaxChoosableInteger: 4, DesiredTotal: 11, Expected: false) },
            { new CanWinExample(MaxChoosableInteger: 4, DesiredTotal: 6, Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanWinByBruteForceRecursion_LeetCodeExamples_MatchesExpectedOutcome(CanWinExample example)
    {
        var canWin = CanIWinSolution.CanWinByBruteForceRecursion(
            example.MaxChoosableInteger, example.DesiredTotal);

        Assert.Equal(example.Expected, canWin);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanWinByMemoizedRecursion_LeetCodeExamples_MatchesExpectedOutcome(CanWinExample example)
    {
        var canWin = CanIWinSolution.CanWinByMemoizedRecursion(example.MaxChoosableInteger, example.DesiredTotal);

        Assert.Equal(example.Expected, canWin);
    }

    // One LeetCode example: the largest choosable integer, the total that decides the
    // game, and whether the first player wins. The row names every position - a bare
    // `bool` argument would read as "true" and say nothing about what is true.
    public readonly record struct CanWinExample(int MaxChoosableInteger, int DesiredTotal, bool Expected);
}
