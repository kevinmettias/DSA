using DSAExperimentation.LeetCode.CountSortedVowelStrings;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountSortedVowelStrings;

// Harness only. Both strategies are CountSortedVowelStringsSolution's - the
// brute-force enumeration that builds every sorted vowel string and the memoized
// recurrence that only counts them - asserted against LeetCode's published
// examples plus the two small lengths that catch an off-by-one in the "vowels may
// repeat" rule.
public sealed class CountSortedVowelStringsTests
{
    public static TheoryData<int, int> Examples =>
        new()
        {
            { 1, 5 },
            { 2, 15 },
            { 3, 35 },
            { 4, 70 },
            { 33, 66045 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountVowelStringsByBacktrackingEnumeration_LeetCodeExamples_ReturnsExpectedCount(
        int length, int expected) =>
        Assert.Equal(expected, CountSortedVowelStringsSolution.CountVowelStringsByBacktrackingEnumeration(length));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountVowelStringsByMemoizedRecurrence_LeetCodeExamples_ReturnsExpectedCount(
        int length, int expected) =>
        Assert.Equal(expected, CountSortedVowelStringsSolution.CountVowelStringsByMemoizedRecurrence(length));
}
