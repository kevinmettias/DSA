using DSAExperimentation.LeetCode.CamelcaseMatching;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CamelcaseMatching;

// Harness only. Both strategies are CamelcaseMatchingSolution's - this file pins them
// to LeetCode's three published examples plus the extra-uppercase-letter case, which
// is the one an unanchored regex gets wrong.
public sealed partial class CamelcaseMatchingTests
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
}
