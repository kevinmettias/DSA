using DSAExperimentation.LeetCode.Solutions.BaseballGame;

namespace DSAExperimentation.LeetCode.Solutions.Tests.BaseballGame;

// Harness only. Both strategies are BaseballGameSolution's - this file pins them
// to a record of plain scores and to one that exercises "D" and "+" after a
// negative score. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class BaseballGameSolutionTests
{
    public static TheoryData<string[], int> Cases =>
        new()
        {
            // Plain scores only: 1 + 2 + 3.
            { ["1", "2", "3"], 6 },
            // -3, its double -6, 9, and -6 + 9 = 3, summing to 3.
            { ["-3", "D", "9", "+"], 3 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CalPointsByManualArrayCursor_Cases_ReturnsSumOfValidScores(
        string[] ops, int expected) =>
        Assert.Equal(expected, BaseballGameSolution.CalPointsByManualArrayCursor(ops));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CalPointsByStackReplay_Cases_ReturnsSumOfValidScores(
        string[] ops, int expected) =>
        Assert.Equal(expected, BaseballGameSolution.CalPointsByStackReplay(ops));
}
