using DSAExperimentation.LeetCode.FindTheIndexOfTheFirstOccurrenceInAString;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheIndexOfTheFirstOccurrenceInAString;

// Harness only: both strategies live in
// FindTheIndexOfTheFirstOccurrenceInAStringSolution and are asserted against the
// same published examples.
public sealed class FindTheIndexOfTheFirstOccurrenceInAStringTests
{
    public static TheoryData<string, string, int> Examples =>
        new()
        {
            { "sadbutsad", "sad", 0 },
            { "leetcode", "leeto", -1 },
            { "mississippi", "issip", 4 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IndexOfByStringIndexOf_LeetCodeExamples_ReturnsFirstMatch(
        string haystack, string needle, int expected) =>
        Assert.Equal(
            expected,
            FindTheIndexOfTheFirstOccurrenceInAStringSolution.IndexOfByStringIndexOf(
                new FindTheIndexOfTheFirstOccurrenceInAStringSolution.Haystack(haystack),
                new FindTheIndexOfTheFirstOccurrenceInAStringSolution.Needle(needle)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IndexOfByRollingHash_LeetCodeExamples_ReturnsFirstMatch(
        string haystack, string needle, int expected) =>
        Assert.Equal(
            expected,
            FindTheIndexOfTheFirstOccurrenceInAStringSolution.IndexOfByRollingHash(
                new FindTheIndexOfTheFirstOccurrenceInAStringSolution.Haystack(haystack),
                new FindTheIndexOfTheFirstOccurrenceInAStringSolution.Needle(needle)));
}
