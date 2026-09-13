using DSAExperimentation.LeetCode.FindAllGoodStrings;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindAllGoodStrings;

// Harness only. Both strategies are FindAllGoodStringsSolution's - the candidate
// enumeration baseline and the KMP-automaton digit DP - pinned here to LeetCode's
// published examples plus two whole-alphabet ranges where the answer is countable
// by hand.
public sealed class FindAllGoodStringsTests
{
    public static TheoryData<int, string, string, string, int> Examples =>
        new()
        {
            { 2, "aa", "da", "b", 51 },
            { 8, "leetcode", "leetgoes", "leet", 0 },
            { 2, "gx", "gz", "x", 2 },
            { 1, "a", "z", "a", 25 },
            { 2, "aa", "zz", "zz", 675 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountGoodStringsByEnumeration_LeetCodeExamples_ReturnsCountExcludingEvilSubstring(
        int n, string s1, string s2, string evil, int expected) =>
        Assert.Equal(expected, FindAllGoodStringsSolution.CountGoodStringsByEnumeration(n, s1, s2, evil));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountGoodStringsByAutomatonDigitDp_LeetCodeExamples_ReturnsCountExcludingEvilSubstring(
        int n, string s1, string s2, string evil, int expected) =>
        Assert.Equal(expected, FindAllGoodStringsSolution.CountGoodStringsByAutomatonDigitDp(n, s1, s2, evil));
}
