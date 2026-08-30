using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Score Words Formed by Letters (LC 1255): a hand-rolled include/skip
// recursion over the word list vs. this repo's own Backtrack.Search closed over
// the identical choose/explore/unchoose steps - the same "hand-specialized
// recursion vs. the generic primitive" comparison PartitionToKEqualSumSubsetsBenchmarks
// already makes for LC 698, here choosing "use this word" vs. "skip it" per
// index instead of "which bucket does this number join." The letter budget is
// set to half of what every word combined would need, so CanInclude genuinely
// fails on some branches instead of every word always fitting - a real
// knapsack-shaped search, not just "sum everything."
[MemoryDiagnoser]
public class MaximumScoreWordsFormedByLettersBenchmarks
{
    private const string Alphabet = "abcdefghijklmnop";

    [Params(8, 14)]
    public int WordCount;

    private string[] _words = null!;
    private char[] _letters = null!;
    private int[] _score = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(4);
        _score = new int[26];

        foreach (var letter in Alphabet)
        {
            _score[letter - 'a'] = random.Next(1, 10);
        }

        _words = new string[WordCount];
        var totalUsage = new int[26];

        for (var i = 0; i < WordCount; i++)
        {
            var length = random.Next(2, 5);
            var chars = new char[length];

            for (var j = 0; j < length; j++)
            {
                chars[j] = Alphabet[random.Next(Alphabet.Length)];
                totalUsage[chars[j] - 'a']++;
            }

            _words[i] = new string(chars);
        }

        var letters = new List<char>();

        for (var c = 0; c < 26; c++)
        {
            for (var n = 0; n < (totalUsage[c] + 1) / 2; n++)
            {
                letters.Add((char)('a' + c));
            }
        }

        _letters = letters.ToArray();
    }

    [Benchmark(Baseline = true)]
    public int NaiveRecursion()
    {
        var available = LetterCounts(_letters);
        var wordCounts = _words.Select(LetterCounts).ToArray();
        var wordScores = _words.Select(WordScore).ToArray();

        return Search(0, available, 0);

        int Search(int index, int[] remaining, int currentScore)
        {
            if (index == _words.Length)
            {
                return currentScore;
            }

            var best = Search(index + 1, remaining, currentScore);
            var counts = wordCounts[index];
            var fits = true;

            for (var c = 0; c < 26 && fits; c++)
            {
                fits = counts[c] <= remaining[c];
            }

            if (!fits)
            {
                return best;
            }

            for (var c = 0; c < 26; c++)
            {
                remaining[c] -= counts[c];
            }

            best = Math.Max(best, Search(index + 1, remaining, currentScore + wordScores[index]));

            for (var c = 0; c < 26; c++)
            {
                remaining[c] += counts[c];
            }

            return best;
        }
    }

    [Benchmark]
    public int BacktrackPrimitive()
    {
        var available = LetterCounts(_letters);
        var state = new State(_words, _score, available);
        var best = 0;

        Backtrack.Search<State, bool>(
            state,
            isSolution: s => s.Index == _words.Length,
            candidates: s => s.Index == _words.Length ? [] : s.CanInclude ? [true, false] : [false],
            choose: (s, include) => s.Choose(include),
            unchoose: (s, include) => s.Unchoose(include),
            onSolution: s => best = Math.Max(best, s.CurrentScore));

        return best;
    }

    private static int[] LetterCounts(IEnumerable<char> chars)
    {
        var counts = new int[26];

        foreach (var c in chars)
        {
            counts[c - 'a']++;
        }

        return counts;
    }

    private int WordScore(string word)
    {
        var total = 0;

        foreach (var c in word)
        {
            total += _score[c - 'a'];
        }

        return total;
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
