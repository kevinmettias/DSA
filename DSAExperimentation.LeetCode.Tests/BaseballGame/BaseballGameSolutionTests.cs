using DSAExperimentation.LeetCode.BaseballGame;

namespace DSAExperimentation.LeetCode.Tests.BaseballGame;

// Harness only. Both strategies are BaseballGameSolution's - this file just
// pins them to LeetCode's published examples.
public sealed partial class BaseballGameSolutionTests
{
    public static TheoryData<string[], int> Examples =>
        new()
        {
            // LeetCode examples 1-3.
            { ["5", "2", "C", "D", "+"], 30 },
            { ["5", "-2", "4", "C", "D", "9", "+", "+"], 27 },
            { ["1", "C"], 0 },

            // Plain scores only: 1 + 2 + 3. Then -3, its double -6, 9, and -6 + 9 = 3,
            // summing to 3.
            { ["1", "2", "3"], 6 },
            { ["-3", "D", "9", "+"], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CalPointsByManualArrayCursor_LeetCodeExamples_ReturnsSumOfValidScores(
        string[] ops, int expected) =>
        Assert.Equal(expected, BaseballGameSolution.CalPointsByManualArrayCursor(ops));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CalPointsByStackReplay_LeetCodeExamples_ReturnsSumOfValidScores(
        string[] ops, int expected) =>
        Assert.Equal(expected, BaseballGameSolution.CalPointsByStackReplay(ops));
}
