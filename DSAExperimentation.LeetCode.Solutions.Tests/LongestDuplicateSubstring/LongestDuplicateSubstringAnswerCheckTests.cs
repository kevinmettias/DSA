using DSAExperimentation.LeetCode.Solutions.LongestDuplicateSubstring;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LongestDuplicateSubstring;

public sealed partial class LongestDuplicateSubstringAnswerCheckTests
{
    public static TheoryData<string, string, string, bool> Answers =>
        new()
        {
            { "banana", "ana", "ana", true },
            { "abcab", "ab", "ab", true },
            { "abcd", "", "", true },
            { "abcab", "ca", "ab", false },
            { "banana", "an", "ana", false },
        };

    [Theory]
    [MemberData(nameof(Answers))]
    public void IsAccepted_Answer_HoldsForARepeatedSubstringAsLongAsTheRecordedOne(string text, string answer, string recorded, bool expected)
        => Assert.Equal(expected, LongestDuplicateSubstringAnswerCheck.IsAccepted(text, answer, recorded));
}
