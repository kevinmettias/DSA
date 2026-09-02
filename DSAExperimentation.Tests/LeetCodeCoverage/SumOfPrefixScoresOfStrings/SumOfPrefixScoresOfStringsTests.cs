using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SumOfPrefixScoresOfStrings;

// LeetCode 2416. Sum of Prefix Scores of Strings: build one merged
// LowercaseTrie<int> over every word (the same bounded-alphabet trie
// MapSumPairsTests/CountingWordsWithAGivenPrefixTests already use), but instead of
// storing a value only at each word's end node the way Set does, walk each word in
// via the trie's already-public Root/Children (the same navigation
// MapSumPairsBenchmarks.WalkTo rehearses) and increment every node's Value by 1 - a
// node's Value ends up counting how many inserted words share that exact prefix.
// answer[i] is then just the sum of those counts along words[i]'s own root-to-leaf
// path. No prefix-score-specific trie code needed, just a different use of the same
// Children/Value slots.
public sealed partial class SumOfPrefixScoresOfStringsTests
{
    [Fact]
    public void SumPrefixScores_LeetCodeExampleOne_ReturnsExpectedScores()
    {
        string[] words = ["abc", "ab", "bc", "b"];

        var scores = SumPrefixScores(words);

        Assert.Equal([5, 4, 3, 2], scores);
    }

    [Fact]
    public void SumPrefixScores_LeetCodeExampleTwo_ReturnsExpectedScores()
    {
        string[] words = ["abcd"];

        var scores = SumPrefixScores(words);

        Assert.Equal([4], scores);
    }

    [Fact]
    public void SumPrefixScores_NoSharedPrefixes_EachWordScoresOnlyItself()
    {
        string[] words = ["a", "b", "c"];

        var scores = SumPrefixScores(words);

        Assert.Equal([1, 1, 1], scores);
    }

    private static int[] SumPrefixScores(string[] words)
    {
        var trie = new LowercaseTrie<int>();

        foreach (var word in words)
        {
            Insert(trie.Root, word);
        }

        var scores = new int[words.Length];

        for (var i = 0; i < words.Length; i++)
        {
            scores[i] = ScoreOf(trie.Root, words[i]);
        }

        return scores;
    }

    private static void Insert(LowercaseTrieNode<int> root, string word)
    {
        var current = root;

        foreach (var ch in word)
        {
            current = current.Children[ch - 'a'] ??= new LowercaseTrieNode<int>();
            current.Value++;
        }
    }

    private static int ScoreOf(LowercaseTrieNode<int> root, string word)
    {
        var current = root;
        var score = 0;

        foreach (var ch in word)
        {
            current = current.Children[ch - 'a']!;
            score += current.Value;
        }

        return score;
    }
}
