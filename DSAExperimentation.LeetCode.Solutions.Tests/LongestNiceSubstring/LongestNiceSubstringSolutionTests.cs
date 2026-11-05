using DSAExperimentation.LeetCode.Solutions.LongestNiceSubstring;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LongestNiceSubstring;

// Harness only. Both strategies are LongestNiceSubstringSolution's -
// FindLongestNiceSubstringByBruteForceSubstrings (previously untested scaffolding
// inlined in the benchmark as its baseline arm) now gets the same cases as
// FindLongestNiceSubstringByDivideAndConquer (previously this file's own private
// helper), so a failure names the strategy that broke.
public sealed partial class LongestNiceSubstringSolutionTests
{
    public static TheoryData<NiceSubstringCase> Cases =>
        new()
        {
            { new NiceSubstringCase(S: "dDzeE", Expected: "dD") },
            { new NiceSubstringCase(S: "cAaBb", Expected: "AaBb") },
            { new NiceSubstringCase(S: "aaaa", Expected: string.Empty) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindLongestNiceSubstringByBruteForceSubstrings_Cases_ReturnsEarliestLongestNiceRun(
        NiceSubstringCase testCase) =>
        Assert.Equal(
            testCase.Expected,
            LongestNiceSubstringSolution.FindLongestNiceSubstringByBruteForceSubstrings(testCase.S));

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindLongestNiceSubstringByDivideAndConquer_Cases_ReturnsEarliestLongestNiceRun(
        NiceSubstringCase testCase) =>
        Assert.Equal(
            testCase.Expected,
            LongestNiceSubstringSolution.FindLongestNiceSubstringByDivideAndConquer(testCase.S));

    // One case as one argument. The input and the answer are both strings, so a
    // two-parameter signature let a row be written with the two swapped and still
    // compile; the fields named at each row below say which is which.
    public readonly record struct NiceSubstringCase(string S, string Expected);
}
