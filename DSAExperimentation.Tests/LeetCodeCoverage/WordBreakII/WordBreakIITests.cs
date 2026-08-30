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
        var trie = new Trie<bool>();
        foreach (var word in wordDict)
        {
            trie.Set(word, true);
        }

        return Memoizer.Memoize<int, List<string>>(0, From);

        List<string> From(int start, Func<int, List<string>> from)
        {
            if (start == s.Length)
            {
                return [string.Empty];
            }

            var sentences = new List<string>();

            for (var end = start + 1; end <= s.Length; end++)
            {
                var piece = s[start..end];

                if (!trie.HasPrefix(piece))
                {
                    break;
                }

                if (!trie.HasKey(piece))
                {
                    continue;
                }

                foreach (var suffix in from(end))
                {
                    sentences.Add(suffix.Length == 0 ? piece : piece + " " + suffix);
                }
            }

            return sentences;
        }
    }
}
