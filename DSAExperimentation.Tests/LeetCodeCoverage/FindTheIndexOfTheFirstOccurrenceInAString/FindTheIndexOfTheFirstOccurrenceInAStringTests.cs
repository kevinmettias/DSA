using DSAExperimentation.LeetCode.FindTheIndexOfTheFirstOccurrenceInAString;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheIndexOfTheFirstOccurrenceInAString;

// Harness only: both strategies live in
// FindTheIndexOfTheFirstOccurrenceInAStringSolution and are asserted against the
// same published examples.
public sealed partial class FindTheIndexOfTheFirstOccurrenceInAStringTests
{
    public static TheoryData<FirstOccurrenceCase> Examples =>
        new()
        {
            { new FirstOccurrenceCase(Haystack: "sadbutsad", Needle: "sad", Expected: 0) },
            { new FirstOccurrenceCase(Haystack: "leetcode", Needle: "leeto", Expected: -1) },
            { new FirstOccurrenceCase(Haystack: "mississippi", Needle: "issip", Expected: 4) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IndexOfByStringIndexOf_LeetCodeExamples_ReturnsFirstMatch(FirstOccurrenceCase example)
    {
        var actual = FindTheIndexOfTheFirstOccurrenceInAStringSolution.IndexOfByStringIndexOf(
            new FindTheIndexOfTheFirstOccurrenceInAStringSolution.Haystack(example.Haystack),
            new FindTheIndexOfTheFirstOccurrenceInAStringSolution.Needle(example.Needle));

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void IndexOfByRollingHash_LeetCodeExamples_ReturnsFirstMatch(FirstOccurrenceCase example)
    {
        var actual = FindTheIndexOfTheFirstOccurrenceInAStringSolution.IndexOfByRollingHash(
            new FindTheIndexOfTheFirstOccurrenceInAStringSolution.Haystack(example.Haystack),
            new FindTheIndexOfTheFirstOccurrenceInAStringSolution.Needle(example.Needle));

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the haystack searched, the needle sought in it, and the index
    // of the first match (-1 when there is none). The two strings are named fields rather
    // than two adjacent `string` parameters, so a row is written `new
    // FirstOccurrenceCase(Haystack: ..., Needle: ...)` and a haystack/needle swap has to be
    // typed out by name instead of falling out of a position the compiler would have
    // accepted either way. Nested because it is only ever used inside this test class - it
    // is this harness's own vocabulary, not a type another file would import.
    public readonly record struct FirstOccurrenceCase(string Haystack, string Needle, int Expected);
}
