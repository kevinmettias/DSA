using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.LeetCode.PrefixAndSuffixSearch;

namespace DSAExperimentation.LeetCode.Tests.PrefixAndSuffixSearch;

// Harness only. Both search strategies are PrefixAndSuffixSearchSolution's - this
// file pins them to LeetCode's published examples, including the shared-match
// tie-break that always resolves to the largest word index. The precomputed index the
// second strategy looks up is asserted on its own, key by key.
public sealed partial class PrefixAndSuffixSearchSolutionTests
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

    // "apple" has six prefixes ("" up to "apple") and six suffixes ("" up to "apple"), and
    // '#' occurs in no word, so the 6 * 6 = 36 joined keys are all distinct and all name
    // word 0; a key whose prefix "apple" lacks is absent.
    [Fact]
    public void BuildPrefixSuffixIndex_SingleWord_StoresEveryPrefixSuffixPairOnce()
    {
        var index = PrefixAndSuffixSearchSolution.BuildPrefixSuffixIndex(["apple"]);
        var stored = new[] { "#", "a#e", "apple#apple" }.Select(key => WordIndexAt(index, key));

        Assert.Equal(36, index.Count);
        Assert.Equal([0, 0, 0], stored);
        Assert.False(index.HasKey("b#e"));
    }

    // LeetCode's three-word dictionary. 36 + 49 + 64 keys for words of length 5, 6 and 7,
    // less the ones two words share: apple and apricot share "#", "a#" and "ap#"; apple and
    // orange "#" and "#e"; orange and apricot "#" - and "#" is all three's, so it was taken
    // away once too often. 149 - 6 + 1 = 144 distinct keys. A later word overwrites an
    // earlier one on a shared key, so "#" and "ap#" name apricot (2) and "#e" orange (1),
    // while "app#" and "a#e" are apple's alone (0).
    [Fact]
    public void BuildPrefixSuffixIndex_LeetCodeDictionary_KeepsTheLargestWordIndexOnASharedKey()
    {
        var index = PrefixAndSuffixSearchSolution.BuildPrefixSuffixIndex(["apple", "orange", "apricot"]);
        var stored = new[] { "#", "ap#", "#e", "app#", "a#e" }.Select(key => WordIndexAt(index, key));

        Assert.Equal(144, index.Count);
        Assert.Equal([2, 2, 1, 0, 0], stored);
        Assert.False(index.HasKey("or#t"));
    }

    private static int WordIndexAt(HashMap<string, int> index, string key)
    {
        var found = index.TryGetValue(key, out var wordIndex);

        Assert.True(found);
        return wordIndex;
    }

    // One LeetCode example: the dictionary, the query's two halves and the largest
    // word index that carries both. The two halves are the same type and are told
    // apart at every construction site by name, so a row reads as the case it is
    // rather than as two strings a caller has to keep in order. Nested because it is
    // only ever used inside this test class - it is this harness's own vocabulary,
    // not a type another file would import.
    public readonly record struct PrefixAndSuffixCase(string[] Words, string Prefix, string Suffix, int ExpectedIndex);
}
