using DSAExperimentation.LeetCode.Solutions.FindTheShortestSuperstring;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindTheShortestSuperstring;

public sealed partial class FindTheShortestSuperstringAnswerCheckTests
{
    public static TheoryData<string[], string, string, bool> Answers =>
        new()
        {
            { ["shortest", "superstring", "string", "abc"], "superstringshortestabc", "superstringshortestabc", true },
            { ["shortest", "superstring", "string", "abc"], "shortestsuperstringabc", "superstringshortestabc", true },
            { ["abc", "bcd", "xyz", "zyx"], "abcdxyzyx", "zyxyzabcd", true },
            { ["abc", "bcd", "xyz", "zyx"], "abcdxyzzyx", "zyxyzabcd", false },
            { ["abc", "bcd", "xyz", "zyx"], "abcdxyzyy", "zyxyzabcd", false },
        };

    [Theory]
    [MemberData(nameof(Answers))]
    public void IsAccepted_Answer_HoldsForAShortestStringHoldingEveryWord(string[] words, string answer, string recorded, bool expected)
        => Assert.Equal(expected, FindTheShortestSuperstringAnswerCheck.IsAccepted(words, answer, recorded));
}
