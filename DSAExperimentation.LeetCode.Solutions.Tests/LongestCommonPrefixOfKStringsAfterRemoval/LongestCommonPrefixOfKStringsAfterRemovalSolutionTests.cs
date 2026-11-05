using DSAExperimentation.Domain.Language.English;
using DSAExperimentation.LeetCode.Solutions.LongestCommonPrefixOfKStringsAfterRemoval;
using PrefixTrie = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.Trie<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.AlphabetSlots<
        DSAExperimentation.Domain.Language.English.EnglishLowercase,
        DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.PackedTrieLayout,
        int>,
    int>;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LongestCommonPrefixOfKStringsAfterRemoval;

// Harness only. Both strategies are LongestCommonPrefixOfKStringsAfterRemovalSolution's.
// CatalogCaseTests holds them to LeetCode's published examples (ARCHITECTURE 17.13), the
// duplicate-heavy first one among them. This file pins them to one prefix that branches,
// whose count is right only if every child's words are summed (no LeetCode example decides
// on such a node), and asserts the counting trie the reduce strategy is handed on its own.
public sealed partial class LongestCommonPrefixOfKStringsAfterRemovalSolutionTests
{
    // "a" opens three words through two children, b with one word and c with two, so its count
    // is the sum over both children: it reaches 3, and qualifies, only while "b" is the word removed.
    public static TheoryData<string[], int, int[]> BranchingPrefixCases =>
        new()
        {
            { ["ab", "ac", "ac", "b"], 3, [0, 0, 0, 1] },
        };

    [Theory]
    [MemberData(nameof(BranchingPrefixCases))]
    public void AnswerByBruteForce_BranchingPrefix_CountsTheWordsUnderEveryChild(
        string[] words, int requiredShareCount, int[] expected)
    {
        var actual = LongestCommonPrefixOfKStringsAfterRemovalSolution.AnswerByBruteForce(words, requiredShareCount);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(BranchingPrefixCases))]
    public void AnswerByReduceTrie_BranchingPrefix_CountsTheWordsUnderEveryChild(
        string[] words, int requiredShareCount, int[] expected)
    {
        var actual = LongestCommonPrefixOfKStringsAfterRemovalSolution.AnswerByReduceTrie(words, requiredShareCount);

        Assert.Equal(expected, actual);
    }

    // LeetCode's first example holds "jump" twice and "run" three times: two distinct
    // keys, each valued with its own copy count, 2 and 3. A prefix such as "ru" is a path
    // through the trie but no key, and the root branches on 'j' and 'r' alone.
    [Fact]
    public void BuildTrie_LeetCodeFirstExample_CountsEachWordsCopiesAtItsEndNode()
    {
        var trie = LongestCommonPrefixOfKStringsAfterRemovalSolution.BuildTrie(["jump", "run", "run", "jump", "run"]);
        var copies = new[] { "jump", "run" }.Select(word => CopiesOf(trie, word));
        var rootLetters = Enumerable.Range(0, EnglishLetters.Size)
            .Where(slot => trie.Root.Children.ChildAt(slot) is not null)
            .Select(EnglishLowercase.LetterAt);

        Assert.Equal(2, trie.Count);
        Assert.Equal([2, 3], copies);
        Assert.False(trie.HasKey("ru"));
        Assert.True(trie.HasPrefix("ru"));
        Assert.Equal(['j', 'r'], rootLetters);
    }

    private static int CopiesOf(PrefixTrie trie, string word)
    {
        var found = trie.TryGetValue(word, out var copies);

        Assert.True(found);
        return copies;
    }
}
