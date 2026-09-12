using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.Set;
using DSAExperimentation.DataStructures.Trie;

namespace DSAExperimentation.LeetCode.ConcatenatedWords;

// LeetCode 472. Concatenated Words: which words in the array can be built by
// concatenating at least two shorter words also drawn from the array?
//
// Both strategies decide "can this one word be segmented, forbidding a single
// piece that consumes the whole word" and apply that check to every word in the
// list. HashSetUnboundedScan is the textbook DP + hash-membership approach - each
// start index rescans every candidate end position with no way to know a prefix
// is already dead. TriePrunedMemoized is the WordBreak/WordBreakII precedent: a
// Trie<bool> lets HasPrefix break the inner loop the moment no dictionary word
// extends the current prefix, and Memoizer caches each start index so a shared
// suffix is only solved once.
internal static class ConcatenatedWordsSolution
{
    public static List<string> FindAllByHashSetScan(string[] words)
    {
        var dictionary = new Set<string>(words);

        return FindAllByHashSetScan(words, dictionary);
    }

    public static List<string> FindAllByHashSetScan(string[] words, Set<string> dictionary) =>
        words.Where(word => CanFormFromOtherWordsByHashSetScan(word, dictionary)).ToList();

    // The naive baseline: a plain HashSet-membership scan (every start index
    // rescans candidate end positions all the way to the word's length, with no
    // way to know a prefix is already dead). Deliberately written without this
    // repo's Trie - it is the arm the composed solution below has to justify
    // itself against.
    private static bool CanFormFromOtherWordsByHashSetScan(string word, Set<string> dictionary)
    {
        var dp = new bool[word.Length + 1];
        dp[0] = true;

        for (var end = 1; end <= word.Length; end++)
        {
            for (var start = 0; start < end; start++)
            {
                if (start == 0 && end == word.Length)
                {
                    // the whole word alone is one piece, not a concatenation of others
                    continue;
                }

                if (dp[start] && dictionary.Has(word[start..end]))
                {
                    dp[end] = true;
                    break;
                }
            }
        }

        return dp[word.Length];
    }

    public static List<string> FindAllByTriePrunedMemo(string[] words)
    {
        var trie = new Trie<bool>();

        foreach (var word in words)
        {
            trie.Set(word, true);
        }

        return FindAllByTriePrunedMemo(words, trie);
    }

    public static List<string> FindAllByTriePrunedMemo(string[] words, Trie<bool> trie) =>
        words.Where(word => CanFormFromOtherWordsByTriePrunedMemo(word, trie)).ToList();

    private static bool CanFormFromOtherWordsByTriePrunedMemo(string word, Trie<bool> trie)
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
