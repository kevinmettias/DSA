using DSAExperimentation.LeetCode.Solutions.StringTransformation;

namespace DSAExperimentation.LeetCode.Solutions.Tests.StringTransformation;

// Harness only: the algorithms live in StringTransformationSolution. One test
// method per strategy over one shared set of cases, so a failure names the
// strategy that broke rather than reporting a disagreement between two anonymous
// arms. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class StringTransformationSolutionTests
{
    public static TheoryData<TransformationCase> Cases =>
        new()
        {
            // No rotation of s spells t at all, so no walk can land on it.
            new TransformationCase("aab", "abc", 5, 0),

            // n = 2: the single legal rotation amount swaps the two characters, so
            // one operation reaches "ba" exactly one way...
            new TransformationCase("ab", "ba", 1, 1),

            // ...and two operations come back to "ab" exactly one way.
            new TransformationCase("ab", "ab", 2, 1),

            // Every rotation of "aaa" spells "aaa", so both l = 1 and l = 2 count.
            new TransformationCase("aaa", "aaa", 1, 2),

            // Period 2 inside n = 4: two of the four positions spell t, each
            // reachable by g(3) = (3^3 + 1) / 4 = 7 walks.
            new TransformationCase("abab", "baba", 3, 14),

            // Same string, period 2: f(2) = 3 walks back to the start plus
            // g(2) = 2 onto the other position spelling s.
            new TransformationCase("abab", "abab", 2, 5),

            // k = 1e12 + 7 exercises the O(log k) closed form; an iterative DP over
            // k could not answer this at all.
            new TransformationCase("abcd", "cdab", 1_000_000_000_007L, 750_475_452),
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void NumberOfWaysByBruteForceRotationCompare_Cases_ReturnsExpectedCount(
        TransformationCase testCase)
    {
        var actual = StringTransformationSolution.NumberOfWaysByBruteForceRotationCompare(
            testCase.S, testCase.T, testCase.K);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void NumberOfWaysByZFunction_Cases_ReturnsExpectedCount(TransformationCase testCase)
    {
        var actual = StringTransformationSolution.NumberOfWaysByZFunction(testCase.S, testCase.T, testCase.K);

        Assert.Equal(testCase.Expected, actual);
    }

    // Nested because it is only ever used inside this test class and has no
    // independent identity: this harness's own vocabulary for one case, naming the
    // source string, the target string, the rotation budget and the expected count.
    // Two adjacent `string` parameters would let a caller transpose the source and
    // the target silently.
    public readonly record struct TransformationCase(string S, string T, long K, int Expected);
}
