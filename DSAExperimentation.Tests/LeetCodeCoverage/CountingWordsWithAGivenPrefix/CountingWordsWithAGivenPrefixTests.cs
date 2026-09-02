using DSAExperimentation.DataStructures.Trie;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountingWordsWithAGivenPrefix;

// LeetCode 2185. Counting Words With a Given Prefix: the same Insert+HasPrefix pairing
// CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceTests already uses (LC 1455) - one word
// inserted alone into this repo's own Trie<bool> per candidate, then HasPrefix(pref) -
// just counting how many words match instead of returning the first matching index.
public sealed partial class CountingWordsWithAGivenPrefixTests
{
    [Fact]
    public void CountWordsWithPrefix_ClassicExampleOne_ReturnsTwo()
    {
        string[] words = ["pay", "attention", "practice", "attend"];

        var count = CountWordsWithPrefix(words, "at");

        Assert.Equal(2, count);
    }

    [Fact]
    public void CountWordsWithPrefix_ClassicExampleTwo_ReturnsZero()
    {
        string[] words = ["leetcode", "win", "loops", "success"];

        var count = CountWordsWithPrefix(words, "code");

        Assert.Equal(0, count);
    }

    [Fact]
    public void CountWordsWithPrefix_PrefixEqualsWholeWord_CountsThatWord()
    {
        string[] words = ["i", "love", "leetcode"];

        var count = CountWordsWithPrefix(words, "i");

        Assert.Equal(1, count);
    }

    [Fact]
    public void CountWordsWithPrefix_PrefixLongerThanAnyWord_ReturnsZero()
    {
        string[] words = ["a", "b", "c"];

        var count = CountWordsWithPrefix(words, "abc");

        Assert.Equal(0, count);
    }

    private static int CountWordsWithPrefix(string[] words, string pref)
    {
        var count = 0;

        foreach (var word in words)
        {
            var trie = new Trie<bool>();
            trie.Set(word, true);

            if (trie.HasPrefix(pref))
            {
                count++;
            }
        }

        return count;
    }
}
