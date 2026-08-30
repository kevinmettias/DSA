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
        => Assert.Equal(4, IsPrefixOfWord("i love eating burger", "burg"));

    [Fact]
    public void IsPrefixOfWord_ClassicExampleTwo_ReturnsTwo()
        => Assert.Equal(2, IsPrefixOfWord("this problem is an easy problem", "pro"));

    [Fact]
    public void IsPrefixOfWord_NoWordHasThatPrefix_ReturnsMinusOne()
        => Assert.Equal(-1, IsPrefixOfWord("i am tired", "you"));

    [Fact]
    public void IsPrefixOfWord_SearchWordEqualsWholeWord_ReturnsThatIndex()
        => Assert.Equal(3, IsPrefixOfWord("i am tired", "tired"));

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
