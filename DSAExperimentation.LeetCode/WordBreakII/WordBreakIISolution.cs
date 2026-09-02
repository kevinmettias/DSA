using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.Trie;

namespace DSAExperimentation.LeetCode.WordBreakII;

// LeetCode 140. Word Break II: every way to segment s into a space-separated
// sequence of dictionary words, not just whether one exists (#139's question).
//
// Both strategies memoize "every sentence obtainable from s[start..]" per start
// index; they differ only in how they test whether a candidate substring could
// still extend into a dictionary word - the same HashSetScan-vs-TrieMemoized split
// as this repo's Word Break (I) coverage.
internal static class WordBreakIISolution
{
    // The textbook answer: a HashSet<string> membership test plus a hand-rolled
    // start-index memo, rescanning every substring s[start..end] all the way to
    // s.Length with no way to know early that a prefix is already dead.
    public static List<string> SentencesByHashSetScan(string s, IList<string> wordDict)
    {
        var words = new HashSet<string>(wordDict);
        var memo = new Dictionary<int, List<string>>();

        return HashSetScanFrom(0, s, words, memo);
    }

    private static List<string> HashSetScanFrom(
        int start, string s, HashSet<string> words, Dictionary<int, List<string>> memo)
    {
        if (memo.TryGetValue(start, out var cached))
        {
            return cached;
        }

        if (start == s.Length)
        {
            return [string.Empty];
        }

        var sentences = new List<string>();

        for (var end = start + 1; end <= s.Length; end++)
        {
            var word = s[start..end];

            if (!words.Contains(word))
            {
                continue;
            }

            foreach (var suffix in HashSetScanFrom(end, s, words, memo))
            {
                AppendSentence(sentences, word, suffix);
            }
        }

        return memo[start] = sentences;
    }

    // This repo's own Trie<bool> screens dictionary prefixes so the scan can break
    // out the moment no dictionary word extends the current piece; Memoizer caches
    // each start index's sentence list so a shared suffix is only solved once.
    public static List<string> SentencesByTrieMemoized(string s, IList<string> wordDict)
    {
        var trie = new Trie<bool>();

        foreach (var word in wordDict)
        {
            trie.Set(word, true);
        }

        return Memoizer.Memoize<int, List<string>>(0, (start, from) => TrieMemoizedFrom(s, trie, start, from));
    }

    private static List<string> TrieMemoizedFrom(string s, Trie<bool> trie, int start, Func<int, List<string>> from)
    {
        if (start == s.Length)
        {
            return [string.Empty];
        }

        var sentences = new List<string>();

        for (var end = start + 1; end <= s.Length; end++)
        {
            var (shouldStop, pieceSentences) = TrieMemoizedPiece(s, trie, start, end, from);

            if (shouldStop)
            {
                break;
            }

            sentences.AddRange(pieceSentences);
        }

        return sentences;
    }

    // Every sentence obtainable by taking s[start..end] as the next word and
    // appending each suffix sentence for the remainder starting at end. ShouldStop
    // mirrors "no dictionary word extends this piece any further, so no longer end
    // can succeed either" - the same early-exit Trie.HasPrefix buys Word Break (I).
    private static (bool ShouldStop, List<string> Sentences) TrieMemoizedPiece(
        string s, Trie<bool> trie, int start, int end, Func<int, List<string>> from)
    {
        var piece = s[start..end];

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
            AppendSentence(sentences, piece, suffix);
        }

        return (false, sentences);
    }

    private static void AppendSentence(List<string> sentences, string word, string suffix) =>
        sentences.Add(suffix.Length == 0 ? word : word + " " + suffix);
}
