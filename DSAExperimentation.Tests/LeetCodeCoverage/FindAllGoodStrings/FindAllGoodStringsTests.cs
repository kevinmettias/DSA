using DSAExperimentation.LeetCode.FindAllGoodStrings;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindAllGoodStrings;

// Harness only. Both strategies are FindAllGoodStringsSolution's - the candidate
// enumeration baseline and the KMP-automaton digit DP - pinned here to LeetCode's
// published examples plus two whole-alphabet ranges where the answer is countable
// by hand.
public sealed class FindAllGoodStringsTests
{
    public static TheoryData<GoodStringRangeCase> Examples =>
        new()
        {
            { new GoodStringRangeCase(N: 2, S1: "aa", S2: "da", Evil: "b", Expected: 51) },
            { new GoodStringRangeCase(N: 8, S1: "leetcode", S2: "leetgoes", Evil: "leet", Expected: 0) },
            { new GoodStringRangeCase(N: 2, S1: "gx", S2: "gz", Evil: "x", Expected: 2) },
            { new GoodStringRangeCase(N: 1, S1: "a", S2: "z", Evil: "a", Expected: 25) },
            { new GoodStringRangeCase(N: 2, S1: "aa", S2: "zz", Evil: "zz", Expected: 675) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountGoodStringsByEnumeration_LeetCodeExamples_ReturnsCountExcludingEvilSubstring(
        GoodStringRangeCase example)
    {
        var actual = FindAllGoodStringsSolution.CountGoodStringsByEnumeration(
            example.N,
            new FindAllGoodStringsSolution.LowerBound(example.S1),
            new FindAllGoodStringsSolution.UpperBound(example.S2),
            new FindAllGoodStringsSolution.ForbiddenSubstring(example.Evil));

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountGoodStringsByAutomatonDigitDp_LeetCodeExamples_ReturnsCountExcludingEvilSubstring(
        GoodStringRangeCase example)
    {
        var actual = FindAllGoodStringsSolution.CountGoodStringsByAutomatonDigitDp(
            example.N,
            new FindAllGoodStringsSolution.LowerBound(example.S1),
            new FindAllGoodStringsSolution.UpperBound(example.S2),
            new FindAllGoodStringsSolution.ForbiddenSubstring(example.Evil));

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the length, the inclusive lower and upper bounds, the
    // substring no good string may contain, and the count LeetCode says is left. The
    // three strings are the same type and none of them is interchangeable with another,
    // so the row names which is which. Nested because it is only ever used inside this
    // test class - it is this harness's own vocabulary, not a type another file would
    // import.
    public readonly record struct GoodStringRangeCase(
        int N, string S1, string S2, string Evil, int Expected);
}
