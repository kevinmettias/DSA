using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.LeetCode.Solutions.PrefixAndSuffixSearch;

namespace DSAExperimentation.LeetCode.Solutions.Tests.PrefixAndSuffixSearch;

// Harness only. Both search strategies are PrefixAndSuffixSearchSolution's - these
// rows query the WordFilter each one's CreateBy factory makes: a prefix no word has,
// a match on a later word, the earlier word's own match, and a prefix and a suffix
// that belong to different words. The precomputed index the second strategy looks
// up, PrecomputedHashMap.BuildPrefixSuffixIndex's, is asserted on its own, key by
// key. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class PrefixAndSuffixSearchSolutionTests
{
    public static TheoryData<PrefixAndSuffixCase> Cases =>
        new()
        {
            { new PrefixAndSuffixCase(Words: ["apple"], Prefix: "b", Suffix: "e", ExpectedIndex: -1) },
            { new PrefixAndSuffixCase(Words: ["apple", "orange", "apricot"], Prefix: "ap", Suffix: "t", ExpectedIndex: 2) },
            { new PrefixAndSuffixCase(Words: ["apple", "orange", "apricot"], Prefix: "app", Suffix: "e", ExpectedIndex: 0) },
            { new PrefixAndSuffixCase(Words: ["apple", "orange", "apricot"], Prefix: "or", Suffix: "t", ExpectedIndex: -1) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CreateByLinearScan_Cases_ReturnsLargestMatchingIndex(PrefixAndSuffixCase testCase)
    {
        var filter = PrefixAndSuffixSearchSolution.CreateByLinearScan(testCase.Words);
        var actual = filter.Search(new SearchPrefix(testCase.Prefix), new SearchSuffix(testCase.Suffix));

        Assert.Equal(testCase.ExpectedIndex, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CreateByPrecomputedHashMap_Cases_ReturnsLargestMatchingIndex(PrefixAndSuffixCase testCase)
    {
        var filter = PrefixAndSuffixSearchSolution.CreateByPrecomputedHashMap(testCase.Words);
        var actual = filter.Search(new SearchPrefix(testCase.Prefix), new SearchSuffix(testCase.Suffix));

        Assert.Equal(testCase.ExpectedIndex, actual);
    }

    // "apple" has six prefixes ("" up to "apple") and six suffixes ("" up to "apple"), and
    // '#' occurs in no word, so the 6 * 6 = 36 joined keys are all distinct and all name
    // word 0; a key whose prefix "apple" lacks is absent.
    [Fact]
    public void PrecomputedHashMap_PrefixSuffixIndexOfOneWord_StoresEveryPrefixSuffixPairOnce()
    {
        var index = PrefixAndSuffixSearchSolution.PrecomputedHashMap.BuildPrefixSuffixIndex(["apple"]);
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
    public void PrecomputedHashMap_PrefixSuffixIndexOfLeetCodeDictionary_KeepsTheLargestWordIndexOnASharedKey()
    {
        var index = PrefixAndSuffixSearchSolution.PrecomputedHashMap.BuildPrefixSuffixIndex(["apple", "orange", "apricot"]);
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

    // One case: the dictionary, the query's two halves and the largest word index that
    // carries both. The two halves are the same type and are told apart at every
    // construction site by name, so a row reads as the case it is rather than as two
    // strings a caller has to keep in order. Nested because it is only ever used inside
    // this test class - it is this harness's own vocabulary, not a type another file
    // would import.
    public readonly record struct PrefixAndSuffixCase(string[] Words, string Prefix, string Suffix, int ExpectedIndex);
}
