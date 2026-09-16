using DSAExperimentation.LeetCode.PrefixAndSuffixSearch;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PrefixAndSuffixSearch;

// Harness only. Both search strategies are PrefixAndSuffixSearchSolution's - this
// file just pins them to LeetCode's published examples, including the shared-match
// tie-break that always resolves to the largest word index.
public sealed class PrefixAndSuffixSearchTests
{
    public static TheoryData<PrefixAndSuffixCase> Examples =>
        new()
        {
            { new PrefixAndSuffixCase(Words: ["apple"], Prefix: "a", Suffix: "e", ExpectedIndex: 0) },
            { new PrefixAndSuffixCase(Words: ["apple"], Prefix: "b", Suffix: "e", ExpectedIndex: -1) },
            { new PrefixAndSuffixCase(Words: ["apple", "orange", "apricot"], Prefix: "ap", Suffix: "t", ExpectedIndex: 2) },
            { new PrefixAndSuffixCase(Words: ["apple", "orange", "apricot"], Prefix: "app", Suffix: "e", ExpectedIndex: 0) },
            { new PrefixAndSuffixCase(Words: ["apple", "orange", "apricot"], Prefix: "or", Suffix: "t", ExpectedIndex: -1) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SearchByLinearScan_LeetCodeExamples_ReturnsLargestMatchingIndex(PrefixAndSuffixCase example)
    {
        var actual = PrefixAndSuffixSearchSolution.SearchByLinearScan(
            example.Words, new SearchPrefix(example.Prefix), new SearchSuffix(example.Suffix));

        Assert.Equal(example.ExpectedIndex, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SearchByPrecomputedHashMap_LeetCodeExamples_ReturnsLargestMatchingIndex(PrefixAndSuffixCase example)
    {
        var actual = PrefixAndSuffixSearchSolution.SearchByPrecomputedHashMap(
            example.Words, new SearchPrefix(example.Prefix), new SearchSuffix(example.Suffix));

        Assert.Equal(example.ExpectedIndex, actual);
    }

    // One LeetCode example: the dictionary, the query's two halves and the largest
    // word index that carries both. The two halves are the same type and are told
    // apart at every construction site by name, so a row reads as the case it is
    // rather than as two strings a caller has to keep in order. Nested because it is
    // only ever used inside this test class - it is this harness's own vocabulary,
    // not a type another file would import.
    public readonly record struct PrefixAndSuffixCase(string[] Words, string Prefix, string Suffix, int ExpectedIndex);
}
