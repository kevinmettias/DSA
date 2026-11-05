using DSAExperimentation.Algorithms.Metrics;
using DSAExperimentation.LeetCode.Solutions.LongestCommonSuffixQueries;
using SuffixTrie = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.Trie<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.AlphabetSlots<
        DSAExperimentation.Domain.Language.English.EnglishLowercase,
        DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.DenseTrieLayout,
        int>,
    int>;
using SuffixNode = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.TrieNode<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.AlphabetSlots<
        DSAExperimentation.Domain.Language.English.EnglishLowercase,
        DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.DenseTrieLayout,
        int>,
    int>;
using SuffixTopology = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.TrieTopology<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.AlphabetSlots<
        DSAExperimentation.Domain.Language.English.EnglishLowercase,
        DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.DenseTrieLayout,
        int>,
    int>;
using SuffixChildren = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.TrieChildren<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.AlphabetSlots<
        DSAExperimentation.Domain.Language.English.EnglishLowercase,
        DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.DenseTrieLayout,
        int>,
    int>;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LongestCommonSuffixQueries;

// Harness only. The suffix trie itself is LongestCommonSuffixQueriesSolution's.
// CatalogCaseTests holds both strategies to LeetCode's published examples (ARCHITECTURE
// 17.13), the "xyz" case that reaches the trie's root value among them; this file pins the
// trie to the best word it should record under every reversed suffix.
public sealed partial class LongestCommonSuffixQueriesSolutionTests
{
    // LeetCode's first container, words reversed: "dcba" (0), "dcb" (1), "dcbx" (2). The
    // empty key holds the shortest word, 1. "abcd" claims d, dc, dcb and dcba first; the
    // strictly shorter "bcd" takes d, dc and dcb over; "xbcd" is longer than "bcd", so it
    // adds only dcbx. Six nodes in all, each holding a word.
    [Fact]
    public void BuildSuffixTrie_LeetCodeFirstExample_RecordsTheShortestWordUnderEachReversedSuffix()
    {
        var trie = LongestCommonSuffixQueriesSolution.BuildSuffixTrie(["abcd", "bcd", "xbcd"]);
        var bestWords = new[] { "", "d", "dc", "dcb", "dcba", "dcbx" }.Select(key => BestWordAt(trie, key));

        Assert.Equal(6, TreeMetrics.Size<SuffixNode, SuffixTopology, SuffixChildren>(trie.Root));
        Assert.Equal([1, 1, 1, 1, 0, 2], bestWords);
    }

    // Two words of equal length share the suffix "b": only a strictly shorter word
    // overwrites a key, so the tie at "" and at "b" keeps the earlier index, 0, and
    // each word alone owns its full reversal, "ba" and "bc".
    [Fact]
    public void BuildSuffixTrie_EqualLengthTie_KeepsTheEarlierIndex()
    {
        var trie = LongestCommonSuffixQueriesSolution.BuildSuffixTrie(["ab", "cb"]);
        var bestWords = new[] { "", "b", "ba", "bc" }.Select(key => BestWordAt(trie, key));

        Assert.Equal(4, TreeMetrics.Size<SuffixNode, SuffixTopology, SuffixChildren>(trie.Root));
        Assert.Equal([0, 0, 0, 1], bestWords);
    }

    // A query that is a whole word must be followed to its first letter: "abcd" shares all four
    // letters with "abcd" (0), while one letter short it shares "bcd" with the shorter "bcd" (1).
    // None of LeetCode's examples tells the two apart.
    [Fact]
    public void FindIndicesByTrie_QueryThatIsAWholeWord_FollowsItToItsFirstLetter() =>
        Assert.Equal([0], LongestCommonSuffixQueriesSolution.FindIndicesByTrie(["abcd", "bcd"], ["abcd"]));

    private static int BestWordAt(SuffixTrie trie, string reversedSuffix)
    {
        var found = trie.TryGetValue(reversedSuffix, out var wordIndex);

        Assert.True(found);
        return wordIndex;
    }
}
