using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.MaximumScoreWordsFormedByLetters;

// LeetCode 1255. Maximum Score Words Formed by Letters: pick any subset of words
// that the shared letter pool can spell, and report the best total letter score.
//
// words.length <= 14 makes this a small enough choose/explore/unchoose search over
// "include this word or skip it" - the same include/skip decision tree
// Backtrack.Search's own doc comment names for Subsets/Combination Sum, except
// every leaf (not just IsSolution ones) is a candidate answer, so OnSolution just
// tracks the running best instead of materializing a result list.
//
// Both strategies gate the "include" choice on the word's own letters still fitting
// the shared, mutated budget - the same shrinking-resource-pool shape
// PartitionToKEqualSumSubsetsSolution's bucket capacity already uses, per letter
// instead of per sum. They differ only in whether the recursion is hand-written or
// closed over this repo's Backtrack primitive.
internal static class MaximumScoreWordsFormedByLettersSolution
{
    private const int AlphabetSize = 26;

    // The textbook recursion: a hand-written include/skip DFS that undoes its own
    // letter spend on the way back up. Deliberately written without this repo's
    // Backtrack primitive - it is the arm the composed solution below has to
    // justify itself against.
    public static int MaxScoreWordsByNaiveRecursion(string[] words, char[] letters, int[] score)
    {
        var remaining = LetterCounts(letters);
        var wordCounts = words.Select(LetterCounts).ToArray();
        var wordScores = words.Select(word => WordScore(word, score)).ToArray();

        return SearchByInclusion(0, remaining, 0, new WordInventory(wordCounts, wordScores));
    }

    private static int WordScore(string word, int[] score)
    {
        var total = 0;

        foreach (var c in word)
        {
            total += score[c - 'a'];
        }

        return total;
    }

    // This repo's own Backtrack.Search, closed over the identical choose/explore/
    // unchoose steps the naive recursion writes out by hand: the shared letter
    // budget is the mutated state, "include this word" is the choice, and every
    // leaf reports its running score so OnSolution can keep the maximum.
    public static int MaxScoreWordsByBacktrackSearch(string[] words, char[] letters, int[] score)
    {
        var state = new WordChoiceState(words, score, LetterCounts(letters));
        var best = 0;

        Backtrack.Search<WordChoiceState, bool>(
            state,
            isSolution: s => s.Index == words.Length,
            candidates: s => CandidatesFor(s, words.Length),
            choose: (s, include) => s.Choose(include),
            unchoose: (s, include) => s.Unchoose(include),
            onSolution: s => best = Math.Max(best, s.CurrentScore));

        return best;
    }

    // The choices at one node: none once every word has been decided, otherwise
    // include-or-skip while the current word still fits the shared letter budget, and
    // skip alone when it does not. CanInclude is only read for an undecided node, which
    // is what keeps it from indexing past the last word.
    private static IEnumerable<bool> CandidatesFor(WordChoiceState state, int wordCount)
    {
        if (state.Index == wordCount)
        {
            return [];
        }

        if (state.CanInclude)
        {
            return [true, false];
        }

        return [false];
    }

    private static int SearchByInclusion(int index, int[] remaining, int currentScore, WordInventory words)
    {
        if (index == words.Scores.Length)
        {
            return currentScore;
        }

        var skipped = SearchByInclusion(index + 1, remaining, currentScore, words);
        var counts = words.Counts[index];

        if (!CanFit(counts, remaining))
        {
            return skipped;
        }

        ApplyCounts(remaining, counts, LetterBudget.Spend);
        var included = SearchByInclusion(index + 1, remaining, currentScore + words.Scores[index], words);
        ApplyCounts(remaining, counts, LetterBudget.Restore);

        return Math.Max(skipped, included);
    }

    // Whether one word's own letter counts all fit inside what the shared pool has left.
    private static bool CanFit(int[] counts, int[] remaining)
    {
        for (var c = 0; c < AlphabetSize; c++)
        {
            if (counts[c] > remaining[c])
            {
                return false;
            }
        }

        return true;
    }

    private static void ApplyCounts(int[] remaining, int[] counts, LetterBudget change)
    {
        var sign = change == LetterBudget.Spend ? -1 : 1;

        for (var c = 0; c < AlphabetSize; c++)
        {
            remaining[c] += sign * counts[c];
        }
    }

    private static int[] LetterCounts(IEnumerable<char> chars)
    {
        var counts = new int[AlphabetSize];

        foreach (var c in chars)
        {
            counts[c - 'a']++;
        }

        return counts;
    }

    private readonly record struct WordInventory(int[][] Counts, int[] Scores);

    // One word index plus the shared, mutated letter budget. Unchoose is Choose's
    // exact inverse, which is what lets Candidates re-read CanInclude lazily.
    private sealed class WordChoiceState
    {
        private readonly int[][] _wordCounts;
        private readonly int[] _wordScores;
        private readonly int[] _available;

        public int Index { get; private set; }

        public int CurrentScore { get; private set; }

        public bool CanInclude => CanFit(_wordCounts[Index], _available);

        public WordChoiceState(string[] words, int[] score, int[] available)
        {
            _available = available;
            _wordCounts = new int[words.Length][];
            _wordScores = new int[words.Length];

            for (var i = 0; i < words.Length; i++)
            {
                _wordCounts[i] = LetterCounts(words[i]);
                _wordScores[i] = WordScore(words[i], score);
            }
        }

        public void Choose(bool include)
        {
            if (include)
            {
                ApplyCounts(_available, _wordCounts[Index], LetterBudget.Spend);
                CurrentScore += _wordScores[Index];
            }

            Index++;
        }

        public void Unchoose(bool include)
        {
            Index--;

            if (include)
            {
                ApplyCounts(_available, _wordCounts[Index], LetterBudget.Restore);
                CurrentScore -= _wordScores[Index];
            }
        }
    }

    // Which direction a shared letter-budget adjustment runs: Spend takes the word's own
    // letter counts back out of the pool, Restore puts them back on the way up.
    private enum LetterBudget
    {
        Spend,
        Restore,
    }
}
