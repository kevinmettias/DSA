namespace DSAExperimentation.Tests.LeetCodeCoverage.CamelcaseMatching;

// LeetCode 1023. Camelcase Matching: a plain two-pointer subsequence scan per
// query against the pattern - the same "no repo data structure actually needed"
// shape ValidPalindromeTests already uses. A query matches when every pattern
// character shows up in order (an ordinary subsequence check) and every
// uppercase letter the query itself contributes lines up with the pattern at
// that exact position too - lowercase letters are the only ones a query may
// drop in between.
public sealed partial class CamelcaseMatchingTests
{
    [Fact]
    public void CamelMatch_ClassicExample_FlagsQueriesThatReduceToPattern()
    {
        string[] queries = ["FooBar", "FooBarTest", "FootBall", "FrameBuffer", "ForceFeedBack"];

        var matches = CamelMatch(queries, "FB");

        Assert.Equal([true, false, true, true, false], matches);
    }

    [Fact]
    public void CamelMatch_ExtraUppercaseLetterInQuery_NeverMatches()
    {
        string[] queries = ["FootBall", "FootBALL", "Foot"];

        var matches = CamelMatch(queries, "FoBa");

        Assert.Equal([true, false, false], matches);
    }

    private static bool[] CamelMatch(string[] queries, string pattern)
    {
        var results = new bool[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            results[i] = Matches(queries[i], pattern);
        }

        return results;
    }

    private static bool Matches(string query, string pattern)
    {
        var p = 0;

        foreach (var c in query)
        {
            if (p < pattern.Length && pattern[p] == c)
            {
                p++;
                continue;
            }

            if (char.IsUpper(c))
            {
                return false;
            }
        }

        return p == pattern.Length;
    }
}
