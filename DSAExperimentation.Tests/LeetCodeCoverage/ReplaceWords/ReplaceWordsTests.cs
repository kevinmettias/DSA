using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ReplaceWords;

// LeetCode 648. Replace Words: build one LowercaseTrie<bool> from the dictionary of
// roots, then walk it character-by-character for each sentence word, stopping at the
// first HasValue node reached - the shortest matching root wins because the walk
// visits nodes in root-to-leaf order, exactly LowercaseTrieTests' own "WalkTo"
// pattern applied to a real problem instead of just verifying the witness.
public sealed partial class ReplaceWordsTests
{
    [Fact]
    public void ReplaceWords_ClassicExample_ReplacesEachWordWithItsShortestRoot()
    {
        string[] dictionary = ["cat", "bat", "rat"];
        var sentence = "the cattle was rattled by the battery";

        var result = ReplaceWords(dictionary, sentence);

        Assert.Equal("the cat was rat by the bat", result);
    }

    [Fact]
    public void ReplaceWords_NoRootMatches_LeavesWordUnchanged()
    {
        string[] dictionary = ["a", "b", "c"];
        var sentence = "aadsfasf absbs bbab cadsfafs";

        var result = ReplaceWords(dictionary, sentence);

        Assert.Equal("a a b c", result);
    }

    private static string ReplaceWords(string[] dictionary, string sentence)
    {
        var trie = new LowercaseTrie<bool>();

        foreach (var root in dictionary)
        {
            trie.Set(root, true);
        }

        var words = sentence.Split(' ');

        for (var i = 0; i < words.Length; i++)
        {
            words[i] = ShortestRootPrefix(trie, words[i]);
        }

        return string.Join(' ', words);
    }

    private static string ShortestRootPrefix(LowercaseTrie<bool> trie, string word)
    {
        var current = trie.Root;

        for (var i = 0; i < word.Length; i++)
        {
            var next = current.Children[word[i] - 'a'];

            if (next is null)
            {
                return word;
            }

            current = next;

            if (current.HasValue)
            {
                return word[..(i + 1)];
            }
        }

        return word;
    }
}
