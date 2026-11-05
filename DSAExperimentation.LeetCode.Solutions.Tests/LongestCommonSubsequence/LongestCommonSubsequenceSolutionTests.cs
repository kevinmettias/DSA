using DSAExperimentation.LeetCode.Solutions.LongestCommonSubsequence;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LongestCommonSubsequence;

// Harness only. Both strategies are LongestCommonSubsequenceSolution's -
// LengthByTabulation (previously untested scaffolding inlined in the benchmark as its
// baseline arm) now gets the same cases as LengthByMemoizedSuffixPairDp
// (previously the test's own private helper), so a failure names the strategy that
// broke.
public sealed partial class LongestCommonSubsequenceSolutionTests
{
    public static TheoryData<LcsCase> Cases =>
        new()
        {
            { new LcsCase(Text1: "abcba", Text2: "abcbcba", Expected: 5) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void LengthByTabulation_Cases_ReturnsLcsLength(LcsCase testCase)
    {
        var actual = LongestCommonSubsequenceSolution.LengthByTabulation(testCase.Text1, testCase.Text2);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void LengthByMemoizedSuffixPairDp_Cases_ReturnsLcsLength(LcsCase testCase)
    {
        var actual = LongestCommonSubsequenceSolution.LengthByMemoizedSuffixPairDp(testCase.Text1, testCase.Text2);

        Assert.Equal(testCase.Expected, actual);
    }

    // One case as one argument. Both texts are strings, so a two-parameter signature
    // let a row be written with the two swapped and still compile; the fields named at
    // each row below say which text is which.
    public readonly record struct LcsCase(string Text1, string Text2, int Expected);
}
