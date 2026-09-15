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
    public static List<string> SentencesByHashSetScan(string source, IList<string> wordDict)
    {
        var words = new HashSet<string>(wordDict);
        var memo = new Dictionary<int, List<string>>();

        return HashSetScanFrom(0, source, words, memo);
    }

    // This repo's own Trie<bool> screens dictionary prefixes so the scan can break
    // out the moment no dictionary word extends the current piece; Memoizer caches
    // each start index's sentence list so a shared suffix is only solved once.
    public static List<string> SentencesByTrieMemoized(string source, IList<string> wordDict)
    {
        var trie = new Trie<bool>();

        foreach (var word in wordDict)
        {
            trie.Set(word, true);
        }

        return Memoizer.Memoize<int, List<string>>(0, new SentencesFromEveryIndex(source, trie));
    }

    // The recurrence, named: a start index yields every sentence built by taking one
    // dictionary word as the next piece and appending every suffix sentence for the
    // remainder, with the exhausted string yielding the empty sentence as its base
    // case. The string and the trie screening its prefixes belong to the caller and
    // never vary during a run, so they travel in as constructor state.
    private sealed class SentencesFromEveryIndex(string source, Trie<bool> trie)
        : IRecurrence<int, List<string>>
    {
        /// <inheritdoc/>
        public List<string> Replay(int start, IRecurrence<int, List<string>> rest)
        {
            if (start == source.Length)
            {
                return [string.Empty];
            }

            var sentences = new List<string>();

            for (var end = start + 1; end <= source.Length; end++)
            {
                var (shouldStop, pieceSentences) = TrieMemoizedPiece((start, end), rest);

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
        // The two indices are the span of s under test, so they travel as one.
        private (bool ShouldStop, List<string> Sentences) TrieMemoizedPiece(
            (int Start, int End) span, IRecurrence<int, List<string>> rest)
        {
            var piece = source[span.Start..span.End];

            if (!trie.HasPrefix(piece))
            {
                return (true, []);
            }

            if (!trie.HasKey(piece))
            {
                return (false, []);
            }

            var sentences = new List<string>();

            foreach (var suffix in rest.Replay(span.End, rest))
            {
                AppendSentence(sentences, new DictionaryWord(piece), new SuffixSentence(suffix));
            }

            return (false, sentences);
        }
    }

    private static List<string> HashSetScanFrom(
        int start, string source, HashSet<string> words, Dictionary<int, List<string>> memo)
    {
        if (memo.TryGetValue(start, out var cached))
        {
            return cached;
        }

        if (start == source.Length)
        {
            return [string.Empty];
        }

        return memo[start] = ScanPiecesFrom(start, source, words, memo);
    }

    // Every sentence obtainable from s[start..] by taking one dictionary word as the
    // next piece: for each end index whose piece is a word, every sentence from the
    // remainder starting at end extends it and joins the list. This is the scan the
    // memo above caches, so it recurses back through HashSetScanFrom rather than here.
    private static List<string> ScanPiecesFrom(
        int start, string source, HashSet<string> words, Dictionary<int, List<string>> memo)
    {
        var sentences = new List<string>();

        for (var end = start + 1; end <= source.Length; end++)
        {
            var word = source[start..end];

            if (!words.Contains(word))
            {
                continue;
            }

            foreach (var suffix in HashSetScanFrom(end, source, words, memo))
            {
                AppendSentence(sentences, new DictionaryWord(word), new SuffixSentence(suffix));
            }
        }

        return sentences;
    }

    // The two halves a joined sentence is built from, named for what each is in this
    // problem rather than left as two adjacent `string` positions a caller could hand
    // over the wrong way round with the compiler none the wiser: `word` is the single
    // dictionary word this piece contributes, `suffix` the sentence covering the rest
    // of the string, which the word is prepended to.
    private static void AppendSentence(List<string> sentences, DictionaryWord word, SuffixSentence suffix) =>
        sentences.Add(suffix.Text.Length == 0 ? word.Text : $"{word.Text} {suffix.Text}");

    internal readonly record struct DictionaryWord(string Text);

    internal readonly record struct SuffixSentence(string Text);
}
