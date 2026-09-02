using DSAExperimentation.DataStructures.Trie;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CheckIfAWordOccursAsAPrefixOfAnyWordInASentence;

// LeetCode 1455. Check If a Word Occurs As a Prefix of Any Word in a Sentence: for
// each sentence word in order, insert it alone into this repo's own Trie<bool> and
// ask HasPrefix(searchWord) - exactly ImplementTrieTests' "insert 'apple', then
// HasPrefix('app') is true" behavior (LC 208), just re-run per candidate word until
// the first match. Returns the first 1-indexed word position, or -1.
public sealed partial class CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceTests
{
    [Fact]
    public void IsPrefixOfWord_ClassicExampleOne_ReturnsFour()
    {
        var index = IsPrefixOfWord("i love eating burger", "burg");

        Assert.Equal(4, index);
    }

    [Fact]
    public void IsPrefixOfWord_ClassicExampleTwo_ReturnsTwo()
    {
        var index = IsPrefixOfWord("this problem is an easy problem", "pro");

        Assert.Equal(2, index);
    }

    [Fact]
    public void IsPrefixOfWord_NoWordHasThatPrefix_ReturnsMinusOne()
    {
        var index = IsPrefixOfWord("i am tired", "you");

        Assert.Equal(-1, index);
    }

    [Fact]
    public void IsPrefixOfWord_SearchWordEqualsWholeWord_ReturnsThatIndex()
    {
        var index = IsPrefixOfWord("i am tired", "tired");

        Assert.Equal(3, index);
    }

    private static int IsPrefixOfWord(string sentence, string searchWord)
    {
        var words = sentence.Split(' ');

        for (var i = 0; i < words.Length; i++)
        {
            var trie = new Trie<bool>();
            trie.Set(words[i], true);

            if (trie.HasPrefix(searchWord))
            {
                return i + 1;
            }
        }

        return -1;
    }
}
