using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ShortEncodingOfWords;

// LeetCode 820. Short Encoding of Words: a word never needs its own "#"-terminated
// entry in the reference string once some other, longer word already ends with it
// - so every word starts in this repo's own Set<string>, and every one of its
// proper suffixes gets evicted the instant a longer word is found to contain it.
// Whatever survives is exactly the set of words that need their own encoding.
public sealed partial class ShortEncodingOfWordsTests
{
    [Fact]
    public void MinimumLength_ClassicExample_DropsWordsThatAreSuffixesOfLongerOnes()
    {
        string[] words = ["time", "me", "bell"];

        var length = MinimumLengthEncoding(words);

        Assert.Equal(10, length); // "time#bell#"
    }

    [Fact]
    public void MinimumLength_NoWordIsASuffixOfAnother_KeepsEveryWord()
    {
        string[] words = ["t"];

        var length = MinimumLengthEncoding(words);

        Assert.Equal(2, length); // "t#"
    }

    private static int MinimumLengthEncoding(string[] words)
    {
        var distinctWords = Deduplicate(words);
        var remaining = FindWordsThatAreNotSuffixesOfAnother(distinctWords);

        return SumEncodingLength(distinctWords, remaining);
    }

    private static List<string> Deduplicate(string[] words)
    {
        var distinctWords = new List<string>();
        var seen = new Set<string>();

        foreach (var word in words)
        {
            if (seen.TryAdd(word))
            {
                distinctWords.Add(word);
            }
        }

        return distinctWords;
    }

    // Every word starts as a survivor, then every one of its proper suffixes gets
    // evicted the instant a longer word is found to contain it.
    private static Set<string> FindWordsThatAreNotSuffixesOfAnother(List<string> distinctWords)
    {
        var remaining = new Set<string>();

        foreach (var word in distinctWords)
        {
            remaining.TryAdd(word);
        }

        foreach (var word in distinctWords)
        {
            for (var i = 1; i < word.Length; i++)
            {
                remaining.TryRemove(word[i..]);
            }
        }

        return remaining;
    }

    private static int SumEncodingLength(List<string> distinctWords, Set<string> remaining)
    {
        var length = 0;

        foreach (var word in distinctWords)
        {
            if (remaining.Has(word))
            {
                length += word.Length + 1;
            }
        }

        return length;
    }
}
