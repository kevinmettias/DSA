using DSAExperimentation.LeetCode.Solutions.LongestPalindromicSubstring;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LongestPalindromicSubstring;

public sealed partial class LongestPalindromicSubstringAnswerCheckTests
{
    public static TheoryData<string, string, string, bool> Answers =>
        new()
        {
            { "babad", "aba", "bab", true },
            { "babad", "bab", "bab", true },
            { "cbbd", "bb", "bb", true },
            { "babad", "ba", "bab", false },
            { "babad", "aca", "bab", false },
        };

    [Theory]
    [MemberData(nameof(Answers))]
    public void IsAccepted_Answer_HoldsForAPalindromeInTheTextAsLongAsTheRecordedOne(string text, string answer, string recorded, bool expected)
        => Assert.Equal(expected, LongestPalindromicSubstringAnswerCheck.IsAccepted(text, answer, recorded));
}
