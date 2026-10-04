using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.LongestCommonPrefixOfKStringsAfterRemoval;

namespace DSAExperimentation.LeetCode.Tests.LongestCommonPrefixOfKStringsAfterRemoval;

// Harness only. Both strategies are
// LongestCommonPrefixOfKStringsAfterRemovalSolution's - this file just pins them
// to LeetCode's published examples, including the duplicate-heavy first example
// that exercises per-word multiplicity rather than mere prefix presence. The counting
// trie the reduce strategy is handed is asserted on its own.
public sealed partial class LongestCommonPrefixOfKStringsAfterRemovalSolutionTests
{
    public static TheoryData<string[], int, int[]> Examples =>
        new()
        {
            { ["jump", "run", "run", "jump", "run"], 2, [3, 4, 4, 3, 4] },
            { ["dog", "racer", "car"], 2, [0, 0, 0] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void AnswerByBruteForce_LeetCodeExamples_ReturnsLongestSharedPrefixPerRemoval(
        string[] words, int requiredShareCount, int[] expected)
    {
        var actual = LongestCommonPrefixOfKStringsAfterRemovalSolution.AnswerByBruteForce(words, requiredShareCount);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void AnswerByReduceTrie_LeetCodeExamples_ReturnsLongestSharedPrefixPerRemoval(
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
        var rootLetters = trie.Root.Children
            .Index()
            .Where(slot => slot.Item is not null)
            .Select(slot => (char)('a' + slot.Index));

        Assert.Equal(2, trie.Count);
        Assert.Equal([2, 3], copies);
        Assert.False(trie.HasKey("ru"));
        Assert.True(trie.HasPrefix("ru"));
        Assert.Equal(['j', 'r'], rootLetters);
    }

    private static int CopiesOf(LowercaseTrie<int> trie, string word)
    {
        var found = trie.TryGetValue(word, out var copies);

        Assert.True(found);
        return copies;
    }
}
