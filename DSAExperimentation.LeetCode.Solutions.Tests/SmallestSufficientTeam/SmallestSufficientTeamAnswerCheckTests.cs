using DSAExperimentation.LeetCode.Solutions.SmallestSufficientTeam;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SmallestSufficientTeam;

public sealed partial class SmallestSufficientTeamAnswerCheckTests
{
    public static TheoryData<string[], string[][], int[], int[], bool> Answers =>
        new()
        {
            { ["java", "nodejs", "reactjs"], [["java"], ["nodejs"], ["nodejs", "reactjs"]], [0, 2], [0, 2], true },
            { ["java", "nodejs", "reactjs"], [["java"], ["nodejs"], ["nodejs", "reactjs"]], [2, 0], [0, 2], true },
            { ["java", "nodejs", "reactjs"], [["java"], ["nodejs"], ["nodejs", "reactjs"]], [0, 1, 2], [0, 2], false },
            { ["java", "nodejs", "reactjs"], [["java"], ["nodejs"], ["nodejs", "reactjs"]], [0, 1], [0, 2], false },
            { ["java", "nodejs", "reactjs"], [["java"], ["nodejs"], ["nodejs", "reactjs"]], [2, 2], [0, 2], false },
        };

    [Theory]
    [MemberData(nameof(Answers))]
    public void IsAccepted_Answer_HoldsForASufficientTeamAsSmallAsTheRecordedOne(string[] reqSkills, string[][] people, int[] answer, int[] recorded, bool expected)
        => Assert.Equal(expected, SmallestSufficientTeamAnswerCheck.IsAccepted(reqSkills, people, answer, recorded));
}
