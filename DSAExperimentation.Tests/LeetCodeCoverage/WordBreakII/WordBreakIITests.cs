using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.Trie;

namespace DSAExperimentation.Tests.LeetCodeCoverage.WordBreakII;

// LeetCode 140. Word Break II: same Trie<bool> + Memoizer composition as this
// repo's Word Break (I) coverage, extended to collect every valid sentence
// instead of a single true/false - Trie.HasPrefix still prunes a start index
// as soon as no dictionary word can extend it, and Memoizer caches each start
// index's list of reachable sentences so a shared suffix is only solved once.
public sealed class WordBreakIITests
{
    [Fact]
    public void WordBreak_MemoizedSuffixSearch_ReturnsAllSentences()
    {
        var result = WordBreak("catsanddog", ["cat", "cats", "and", "sand", "dog"]);

        Assert.Equal(["cat sand dog", "cats and dog"], result.OrderBy(s => s).ToArray());
    }

    [Fact]
    public void WordBreak_NoValidSegmentationExists_ReturnsEmpty()
    {
        var result = WordBreak("catsandog", ["cats", "dog", "sand", "and", "cat"]);

        Assert.Empty(result);
    }

    private static IList<string> WordBreak(string s, IList<string> wordDict)
    {
        var trie = BuildTrie(wordDict);
        return Memoizer.Memoize<int, List<string>>(0, (start, from) => From(s, trie, start, from));
    }

    private static Trie<bool> BuildTrie(IList<string> wordDict)
    {
        var trie = new Trie<bool>();
        foreach (var word in wordDict)
        {
            trie.Set(word, true);
        }

        return trie;
    }

    private static List<string> From(string s, Trie<bool> trie, int start, Func<int, List<string>> from)
    {
        if (start == s.Length)
        {
            return [string.Empty];
        }

        var sentences = new List<string>();

        for (var end = start + 1; end <= s.Length; end++)
        {
            var piece = s[start..end];
            var (shouldStop, pieceSentences) = SentencesForPiece(piece, end, trie, from);

            if (shouldStop)
            {
                break;
            }

            sentences.AddRange(pieceSentences);
        }

        return sentences;
    }

    // Every sentence obtainable by taking `piece` as the next word and appending
    // each suffix sentence for the remainder starting at `end` - the self-contained
    // unit From's loop performs once per candidate piece. ShouldStop mirrors the
    // original loop's `break`: no dictionary word extends `piece` any further, so
    // no longer `end` can succeed either.
    private static (bool ShouldStop, List<string> Sentences) SentencesForPiece(
        string piece, int end, Trie<bool> trie, Func<int, List<string>> from)
    {
        if (!trie.HasPrefix(piece))
        {
            return (true, []);
        }

        if (!trie.HasKey(piece))
        {
            return (false, []);
        }

        var sentences = new List<string>();

        foreach (var suffix in from(end))
        {
            sentences.Add(suffix.Length == 0 ? piece : piece + " " + suffix);
        }

        return (false, sentences);
    }
}
