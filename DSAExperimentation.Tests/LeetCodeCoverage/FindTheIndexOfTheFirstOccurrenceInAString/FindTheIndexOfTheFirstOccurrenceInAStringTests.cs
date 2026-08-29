using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheIndexOfTheFirstOccurrenceInAString;

public sealed partial class FindTheIndexOfTheFirstOccurrenceInAStringTests
{
    [Theory]
    [InlineData("sadbutsad", "sad", 0)]
    [InlineData("leetcode", "leeto", -1)]
    [InlineData("mississippi", "issip", 4)]
    public void StrStr_LeetCodeExamples_ReturnsFirstMatch(string haystack, string needle, int expected)
        => Assert.Equal(expected, StrStr(haystack, needle));

    private static int StrStr(string haystack, string needle)
    {
        var matches = RollingHashSearch.FindAll(haystack, needle);
        return matches.Count == 0 ? -1 : matches[0];
    }
}
