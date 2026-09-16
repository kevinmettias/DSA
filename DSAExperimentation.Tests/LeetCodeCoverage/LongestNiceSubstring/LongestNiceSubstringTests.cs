using DSAExperimentation.LeetCode.LongestNiceSubstring;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestNiceSubstring;

// Harness only. Both strategies are LongestNiceSubstringSolution's -
// FindLongestNiceSubstringByBruteForceSubstrings (previously untested scaffolding
// inlined in the benchmark as its baseline arm) now gets the same examples as
// FindLongestNiceSubstringByDivideAndConquer (previously this file's own private
// helper), so a failure names the strategy that broke.
public sealed class LongestNiceSubstringTests
{
    public static TheoryData<NiceSubstringExample> Examples =>
        new()
        {
            { new NiceSubstringExample(S: "YazaAay", Expected: "aAa") },
            { new NiceSubstringExample(S: "Bb", Expected: "Bb") },
            { new NiceSubstringExample(S: "c", Expected: string.Empty) },
            { new NiceSubstringExample(S: "dDzeE", Expected: "dD") },
            { new NiceSubstringExample(S: "abABB", Expected: "abABB") },
            { new NiceSubstringExample(S: "cAaBb", Expected: "AaBb") },
            { new NiceSubstringExample(S: "aaaa", Expected: string.Empty) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindLongestNiceSubstringByBruteForceSubstrings_LeetCodeExamples_ReturnsEarliestLongestNiceRun(
        NiceSubstringExample example) =>
        Assert.Equal(
            example.Expected,
            LongestNiceSubstringSolution.FindLongestNiceSubstringByBruteForceSubstrings(example.S));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindLongestNiceSubstringByDivideAndConquer_LeetCodeExamples_ReturnsEarliestLongestNiceRun(
        NiceSubstringExample example) =>
        Assert.Equal(
            example.Expected,
            LongestNiceSubstringSolution.FindLongestNiceSubstringByDivideAndConquer(example.S));

    // One example as one argument. The input and the answer are both strings, so a
    // two-parameter signature let a row be written with the two swapped and still
    // compile; the fields named at each row below say which is which.
    public readonly record struct NiceSubstringExample(string S, string Expected);
}
