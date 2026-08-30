using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestWordInDictionary;

// LeetCode 720. Longest Word in Dictionary: build one LowercaseTrie<bool> from the
// whole word list (same construction ReplaceWordsTests uses), then walk it from the
// root descending only through nodes whose HasValue is already true - a step is legal
// exactly when the prefix reached so far is itself a complete dictionary word, which
// is this problem's own "built one character at a time" rule - tracking the longest
// (ties broken lexicographically smallest) prefix reached along the way.
public sealed partial class LongestWordInDictionaryTests
{
    [Fact]
    public void LongestWord_ClassicExample_ReturnsFullyBuildableChain()
    {
        string[] words = ["w", "wo", "wor", "worl", "world"];

        var result = LongestWord(words);

        Assert.Equal("world", result);
    }

    [Fact]
    public void LongestWord_TiedLengths_ReturnsLexicographicallySmallest()
    {
        string[] words = ["a", "banana", "app", "appl", "ap", "apply", "apple"];

        var result = LongestWord(words);

        Assert.Equal("apple", result);
    }

    private static string LongestWord(string[] words)
    {
        var trie = new LowercaseTrie<bool>();

        foreach (var word in words)
        {
            trie.Set(word, true);
        }

        var best = "";

        Walk(trie.Root, "");

        return best;

        void Walk(LowercaseTrieNode<bool> node, string prefix)
        {
            if (prefix.Length > 0 &&
                (prefix.Length > best.Length || (prefix.Length == best.Length && string.CompareOrdinal(prefix, best) < 0)))
            {
                best = prefix;
            }

            for (var i = 0; i < LowercaseTrieNode<bool>.AlphabetSize; i++)
            {
                var child = node.Children[i];

                if (child is not null && child.HasValue)
                {
                    Walk(child, prefix + (char)('a' + i));
                }
            }
        }
    }
}
