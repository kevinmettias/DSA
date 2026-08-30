using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.Trie;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ConcatenatedWords;

// LeetCode 472. Concatenated Words: the WordBreak precedent (Trie<bool> dictionary +
// Memoizer-based segmentation DP) run per word against every word in the list,
// with one extra rule: the first piece can never consume the entire word, forcing
// at least two pieces. trie.HasPrefix still prunes dead branches exactly as it does
// in WordBreak, so no branch scans past a point no dictionary word extends.
public sealed partial class ConcatenatedWordsTests
{
    [Fact]
    public void FindAllConcatenatedWords_ClassicExample_ReturnsWordsBuiltFromShorterOnes()
        => Assert.Equal(
            new HashSet<string> { "catsdogcats", "dogcatsdog", "ratcatdogcat" },
            FindAll(["cat", "cats", "catsdogcats", "dog", "dogcatsdog", "hippopotamuses", "rat", "ratcatdogcat"]).ToHashSet());

    [Fact]
    public void FindAllConcatenatedWords_NoWordIsAConcatenation_ReturnsEmpty()
        => Assert.Empty(FindAll(["cat", "dog", "rat"]));

    private static List<string> FindAll(string[] words)
    {
        var trie = new Trie<bool>();
        foreach (var word in words)
        {
            trie.Set(word, true);
        }

        return words.Where(word => CanFormFromOtherWords(word, trie)).ToList();
    }

    private static bool CanFormFromOtherWords(string word, Trie<bool> trie)
    {
        return Memoizer.Memoize<int, bool>(0, From);

        bool From(int start, Func<int, bool> can)
        {
            if (start == word.Length)
            {
                return true;
            }

            for (var end = start + 1; end <= word.Length; end++)
            {
                if (start == 0 && end == word.Length)
                {
                    // the whole word alone is one piece, not a concatenation of others
                    continue;
                }

                var piece = word[start..end];

                if (!trie.HasPrefix(piece))
                {
                    break;
                }

                if (trie.HasKey(piece) && can(end))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
