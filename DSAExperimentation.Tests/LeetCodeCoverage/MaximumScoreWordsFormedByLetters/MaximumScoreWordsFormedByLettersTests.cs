using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumScoreWordsFormedByLetters;

// LeetCode 1255. Maximum Score Words Formed by Letters: words.length <= 14 makes
// this a small enough choose/explore/unchoose search over "include this word or
// skip it" - the same include/skip decision tree Backtrack.Search's own doc
// comment names for Subsets/Combination Sum, except every leaf (not just
// IsSolution ones) is a candidate answer, so OnSolution just tracks the running
// best instead of materializing a result list. CanInclude gates the "include"
// choice on the word's own letters still fitting the shared, mutated budget -
// the same shrinking-resource-pool shape PartitionToKEqualSumSubsetsTests'
// CanPlace already uses for bucket capacity, just per-letter instead of per-sum.
public sealed partial class MaximumScoreWordsFormedByLettersTests
{
    [Fact]
    public void MaxScoreWords_MissingLetterExcludesWordAndBudgetForcesAChoice_ReturnsBestReachableSubset()
    {
        // Available letters give a:2, c:1, d:3, g:1, o:2 and no 't' at all, so
        // "cat" can never be formed - only "dog" (5+0+3=8), "dad" (5+1+5=11) and
        // "good" (3+0+0+5=8) are ever reachable, and the d-budget (3) rules out
        // taking all three together (1+2+1=4 d's needed). The best reachable
        // pair is "dad"+"good" (equivalently "dog"+"dad"), both summing to 19.
        string[] words = ["dog", "cat", "dad", "good"];
        char[] letters = ['a', 'a', 'c', 'd', 'd', 'd', 'g', 'o', 'o'];
        int[] score = [1, 0, 9, 5, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];

        var result = MaxScoreWords(words, letters, score);

        Assert.Equal(19, result);
    }

    [Fact]
    public void MaxScoreWords_LettersExactlyCoverEveryWord_TakesEveryWord()
    {
        string[] words = ["cat", "dog"];
        char[] letters = ['c', 'a', 't', 'd', 'o', 'g'];
        var score = new int[26];

        foreach (var c in letters)
        {
            score[c - 'a'] = 1;
        }

        var result = MaxScoreWords(words, letters, score);

        Assert.Equal(6, result);
    }

    private static int MaxScoreWords(string[] words, char[] letters, int[] score)
    {
        var available = new int[26];

        foreach (var c in letters)
        {
            available[c - 'a']++;
        }

        var state = new State(words, score, available);
        var best = 0;

        Backtrack.Search<State, bool>(
            state,
            isSolution: s => s.Index == words.Length,
            candidates: s => s.Index == words.Length ? [] : s.CanInclude ? [true, false] : [false],
            choose: (s, include) => s.Choose(include),
            unchoose: (s, include) => s.Unchoose(include),
            onSolution: s => best = Math.Max(best, s.CurrentScore));

        return best;
    }

    private sealed class State
    {
        private readonly int[][] _wordCounts;
        private readonly int[] _wordScores;
        private readonly int[] _available;

        public State(string[] words, int[] score, int[] available)
        {
            _available = available;
            _wordCounts = new int[words.Length][];
            _wordScores = new int[words.Length];

            for (var i = 0; i < words.Length; i++)
            {
                var counts = new int[26];
                var wordScore = 0;

                foreach (var c in words[i])
                {
                    counts[c - 'a']++;
                    wordScore += score[c - 'a'];
                }

                _wordCounts[i] = counts;
                _wordScores[i] = wordScore;
            }
        }

        public int Index { get; private set; }

        public int CurrentScore { get; private set; }

        public bool CanInclude
        {
            get
            {
                var counts = _wordCounts[Index];

                for (var c = 0; c < 26; c++)
                {
                    if (counts[c] > _available[c])
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public void Choose(bool include)
        {
            if (include)
            {
                var counts = _wordCounts[Index];

                for (var c = 0; c < 26; c++)
                {
                    _available[c] -= counts[c];
                }

                CurrentScore += _wordScores[Index];
            }

            Index++;
        }

        public void Unchoose(bool include)
        {
            Index--;

            if (include)
            {
                var counts = _wordCounts[Index];

                for (var c = 0; c < 26; c++)
                {
                    _available[c] += counts[c];
                }

                CurrentScore -= _wordScores[Index];
            }
        }
    }
}
