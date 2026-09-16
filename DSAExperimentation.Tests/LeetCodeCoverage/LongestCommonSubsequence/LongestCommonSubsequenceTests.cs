using DSAExperimentation.LeetCode.LongestCommonSubsequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestCommonSubsequence;

// Harness only. Both strategies are LongestCommonSubsequenceSolution's -
// LengthByTabulation (previously untested scaffolding inlined in the benchmark as its
// baseline arm) now gets the same examples as LengthByMemoizedSuffixPairDp
// (previously the test's own private helper), so a failure names the strategy that
// broke.
public sealed class LongestCommonSubsequenceTests
{
    public static TheoryData<LcsExample> Examples =>
        new()
        {
            { new LcsExample(Text1: "abcde", Text2: "ace", Expected: 3) },
            { new LcsExample(Text1: "abc", Text2: "abc", Expected: 3) },
            { new LcsExample(Text1: "abc", Text2: "def", Expected: 0) },
            { new LcsExample(Text1: "abcba", Text2: "abcbcba", Expected: 5) },
            { new LcsExample(Text1: "ezupkr", Text2: "ubmrapg", Expected: 2) },
            { new LcsExample(Text1: "aaaa", Text2: "aa", Expected: 2) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LengthByTabulation_LeetCodeExamples_ReturnsLcsLength(LcsExample example)
    {
        var actual = LongestCommonSubsequenceSolution.LengthByTabulation(example.Text1, example.Text2);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void LengthByMemoizedSuffixPairDp_LeetCodeExamples_ReturnsLcsLength(LcsExample example)
    {
        var actual = LongestCommonSubsequenceSolution.LengthByMemoizedSuffixPairDp(example.Text1, example.Text2);

        Assert.Equal(example.Expected, actual);
    }

    // One example as one argument. Both texts are strings, so a two-parameter signature
    // let a row be written with the two swapped and still compile; the fields named at
    // each row below say which text is which.
    public readonly record struct LcsExample(string Text1, string Text2, int Expected);
}
