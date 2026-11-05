using DSAExperimentation.LeetCode.Solutions.LongestHappyPrefix;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LongestHappyPrefix;

// Harness only. Both strategies are LongestHappyPrefixSolution's - this file pins
// them to a four-character prefix that returns as the suffix and a string with no
// happy prefix at all, one theory per strategy so a failure names the strategy that
// broke. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class LongestHappyPrefixSolutionTests
{
    public static TheoryData<HappyPrefixCase> Cases =>
        new()
        {
            // The whole 4-character run repeats at the end.
            { new HappyPrefixCase(S: "leetcodeleet", Expected: "leet") },

            // No prefix is also a suffix.
            { new HappyPrefixCase(S: "asdf", Expected: string.Empty) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void LongestPrefixByShrinkAndCompare_Cases_ReturnsLongestProperPrefixThatIsAlsoASuffix(
        HappyPrefixCase testCase) =>
        Assert.Equal(testCase.Expected, LongestHappyPrefixSolution.LongestPrefixByShrinkAndCompare(testCase.S));

    [Theory]
    [MemberData(nameof(Cases))]
    public void LongestPrefixByPrefixFunction_Cases_ReturnsLongestProperPrefixThatIsAlsoASuffix(
        HappyPrefixCase testCase) =>
        Assert.Equal(testCase.Expected, LongestHappyPrefixSolution.LongestPrefixByPrefixFunction(testCase.S));

    // One case as one argument. The input and the answer are both strings, so a
    // two-parameter signature let a row be written with the two swapped and still
    // compile; the fields named at each row below say which is which.
    public readonly record struct HappyPrefixCase(string S, string Expected);
}
