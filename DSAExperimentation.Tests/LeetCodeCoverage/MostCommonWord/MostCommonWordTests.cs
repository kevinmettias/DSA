using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MostCommonWord;

// LeetCode 819. Most Common Word: banned words go into this repo's own
// Set<string> for O(1) membership checks, and word counts into a HashMap<string,
// int> - the same count-with-HashMap shape TopKFrequentWords already uses for word
// frequency, with a Set standing in for "words to skip entirely."
public sealed partial class MostCommonWordTests
{
    [Fact]
    public void MostCommon_ClassicExample_SkipsBannedWordDespiteHighestRawFrequency()
    {
        var paragraph = "Bob hit a ball, the hit BALL flew far after it was hit.";
        string[] banned = ["hit"];

        var result = MostCommonWord(paragraph, banned);

        Assert.Equal("ball", result);
    }

    [Fact]
    public void MostCommon_NoBannedWords_ReturnsSingleOccurringWord()
    {
        var result = MostCommonWord("a.", []);

        Assert.Equal("a", result);
    }

    private static string MostCommonWord(string paragraph, string[] banned)
    {
        var bannedWords = new Set<string>();

        foreach (var word in banned)
        {
            bannedWords.TryAdd(word.ToLowerInvariant());
        }

        var counts = new HashMap<string, int>();
        var best = string.Empty;
        var bestCount = 0;

        foreach (var word in Tokenize(paragraph))
        {
            if (bannedWords.Has(word))
            {
                continue;
            }

            counts.TryGetValue(word, out var count);
            count++;
            counts.Set(word, count);

            if (count > bestCount)
            {
                bestCount = count;
                best = word;
            }
        }

        return best;
    }

    // Splits on runs of non-letters, same as LeetCode's own "words separated by
    // spaces and/or punctuation" definition, lower-casing as it goes.
    private static IEnumerable<string> Tokenize(string paragraph)
    {
        var current = new System.Text.StringBuilder();

        foreach (var ch in paragraph)
        {
            if (char.IsLetter(ch))
            {
                current.Append(char.ToLowerInvariant(ch));
                continue;
            }

            if (current.Length > 0)
            {
                yield return current.ToString();
                current.Clear();
            }
        }

        if (current.Length > 0)
        {
            yield return current.ToString();
        }
    }
}
