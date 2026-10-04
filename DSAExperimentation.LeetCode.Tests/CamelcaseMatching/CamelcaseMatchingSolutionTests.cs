using DSAExperimentation.LeetCode.CamelcaseMatching;

namespace DSAExperimentation.LeetCode.Tests.CamelcaseMatching;

// Harness only. Both strategies are CamelcaseMatchingSolution's - this file pins them
// to LeetCode's three published examples plus the extra-uppercase-letter case, which
// is the one an unanchored regex gets wrong. The matcher the regex strategy is handed
// is asserted on its own.
public sealed partial class CamelcaseMatchingSolutionTests
{
    private static readonly string[] ClassicQueries =
        ["FooBar", "FooBarTest", "FootBall", "FrameBuffer", "ForceFeedBack"];

    public static TheoryData<string[], string, bool[]> Examples =>
        new()
        {
            { ClassicQueries, "FB", [true, false, true, true, false] },
            { ClassicQueries, "FoBa", [true, false, true, false, false] },
            { ClassicQueries, "FoBaT", [false, true, false, false, false] },
            { ["FootBall", "FootBALL", "Foot"], "FoBa", [true, false, false] },
            { ["Foo"], "Foo", [true] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CamelMatchByRegex_LeetCodeExamples_FlagsQueriesThatReduceToPattern(
        string[] queries, string pattern, bool[] expected)
    {
        var matches = CamelcaseMatchingSolution.CamelMatchByRegex(queries, pattern);

        Assert.Equal(expected, matches);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CamelMatchByTwoPointerScan_LeetCodeExamples_FlagsQueriesThatReduceToPattern(
        string[] queries, string pattern, bool[] expected)
    {
        var matches = CamelcaseMatchingSolution.CamelMatchByTwoPointerScan(queries, pattern);

        Assert.Equal(expected, matches);
    }

    // LeetCode's first pattern "FB", spliced: a lowercase run before, between and after
    // its two letters, the whole anchored by \A and \z.
    [Fact]
    public void BuildMatcher_LeetCodeFirstPattern_SplicesLowercaseRunsAroundEachLetterAndAnchors()
    {
        var matcher = CamelcaseMatchingSolution.BuildMatcher("FB");

        Assert.Equal(@"\A[a-z]*F[a-z]*B[a-z]*\z", matcher.ToString());
    }

    // The \z anchor is what turns "FooBarTest" away: its trailing "Test" adds an
    // uppercase letter "FB" never asked for, where "FooBar" inserts only lowercase ones.
    [Fact]
    public void BuildMatcher_LeetCodeFirstPattern_RejectsAQueryWithATrailingUppercaseLetter()
    {
        var matcher = CamelcaseMatchingSolution.BuildMatcher("FB");
        var insertsOnlyLowercase = matcher.IsMatch("FooBar");
        var addsAnUppercaseLetter = matcher.IsMatch("FooBarTest");

        Assert.True(insertsOnlyLowercase);
        Assert.False(addsAnUppercaseLetter);
    }
}
