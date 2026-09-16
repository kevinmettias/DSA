using DSAExperimentation.LeetCode.IntegerToEnglishWords;

namespace DSAExperimentation.Tests.LeetCodeCoverage.IntegerToEnglishWords;

// Harness only. Both strategies are IntegerToEnglishWordsSolution's - this file
// just pins them to LeetCode's published examples.
public sealed partial class IntegerToEnglishWordsTests
{
    public static TheoryData<int, string> Examples =>
        new()
        {
            { 0, "Zero" },
            { 123, "One Hundred Twenty Three" },
            { 12345, "Twelve Thousand Three Hundred Forty Five" },
            { 1234567, "One Million Two Hundred Thirty Four Thousand Five Hundred Sixty Seven" },
            {
                2147483647,
                "Two Billion One Hundred Forty Seven Million Four Hundred Eighty Three Thousand Six Hundred Forty Seven"
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumberToWordsByStringPrepend_LeetCodeExamples_ReturnsEnglishWords(int num, string expected) =>
        Assert.Equal(expected, IntegerToEnglishWordsSolution.NumberToWordsByStringPrepend(num));

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumberToWordsByWordStack_LeetCodeExamples_ReturnsEnglishWords(int num, string expected) =>
        Assert.Equal(expected, IntegerToEnglishWordsSolution.NumberToWordsByWordStack(num));
}
