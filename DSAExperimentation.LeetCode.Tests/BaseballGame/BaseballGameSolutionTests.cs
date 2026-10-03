using DSAExperimentation.LeetCode.BaseballGame;

namespace DSAExperimentation.LeetCode.Tests.BaseballGame;

// Harness only. Both strategies are BaseballGameSolution's - this file just
// pins them to LeetCode's published examples.
public sealed partial class BaseballGameSolutionTests
{
    public static TheoryData<string[], int> Examples =>
        new()
        {
            { ["5", "2", "C", "D", "+"], 30 },
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
