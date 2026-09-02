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

            var lookup = new WordLookup(trie, word);
            return TryFindPieceMatch(lookup, start, can);
        }
    }

    private static bool TryFindPieceMatch(WordLookup lookup, int start, Func<int, bool> can)
    {
        for (var end = start + 1; end <= lookup.Word.Length; end++)
        {
            var outcome = EvaluatePiece(lookup, start, end, can);

            if (outcome == PieceOutcome.StopSearching)
            {
                break;
            }

            if (outcome == PieceOutcome.Found)
            {
                return true;
            }
        }

        return false;
    }

    private static PieceOutcome EvaluatePiece(WordLookup lookup, int start, int end, Func<int, bool> can)
    {
        if (start == 0 && end == lookup.Word.Length)
        {
            // the whole word alone is one piece, not a concatenation of others
            return PieceOutcome.Continue;
        }

        var piece = lookup.Word[start..end];

        if (!lookup.Trie.HasPrefix(piece))
        {
            return PieceOutcome.StopSearching;
        }

        return lookup.Trie.HasKey(piece) && can(end) ? PieceOutcome.Found : PieceOutcome.Continue;
    }

    private enum PieceOutcome
    {
        Continue,
        StopSearching,
        Found,
    }

    private readonly record struct WordLookup(Trie<bool> Trie, string Word);
}
