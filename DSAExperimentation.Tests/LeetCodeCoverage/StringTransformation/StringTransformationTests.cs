using DSAExperimentation.LeetCode.StringTransformation;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StringTransformation;

// Harness only: the algorithms live in StringTransformationSolution. One test
// method per strategy over one shared set of examples, so a failure names the
// strategy that broke rather than reporting a disagreement between two anonymous
// arms.
public sealed class StringTransformationTests
{
    public static TheoryData<string, string, long, int> Examples =>
        new()
        {
            // LeetCode's own two examples.
            { "abcd", "cdab", 2, 2 },
            { "ababab", "ababab", 1, 2 },

            // No rotation of s spells t at all, so no walk can land on it.
            { "aab", "abc", 5, 0 },

            // n = 2: the single legal rotation amount swaps the two characters, so
            // one operation reaches "ba" exactly one way...
            { "ab", "ba", 1, 1 },

            // ...and two operations come back to "ab" exactly one way.
            { "ab", "ab", 2, 1 },

            // Every rotation of "aaa" spells "aaa", so both l = 1 and l = 2 count.
            { "aaa", "aaa", 1, 2 },

            // Period 2 inside n = 4: two of the four positions spell t, each
            // reachable by g(3) = (3^3 + 1) / 4 = 7 walks.
            { "abab", "baba", 3, 14 },

            // Same string, period 2: f(2) = 3 walks back to the start plus
            // g(2) = 2 onto the other position spelling s.
            { "abab", "abab", 2, 5 },

            // k = 1e12 + 7 exercises the O(log k) closed form; an iterative DP over
            // k could not answer this at all.
            { "abcd", "cdab", 1_000_000_000_007L, 750_475_452 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumberOfWaysByBruteForceRotationCompare_LeetCodeExamples_ReturnsExpectedCount(
        string s, string t, long k, int expected)
        => Assert.Equal(expected, StringTransformationSolution.NumberOfWaysByBruteForceRotationCompare(s, t, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumberOfWaysByZFunction_LeetCodeExamples_ReturnsExpectedCount(
        string s, string t, long k, int expected)
        => Assert.Equal(expected, StringTransformationSolution.NumberOfWaysByZFunction(s, t, k));
}
