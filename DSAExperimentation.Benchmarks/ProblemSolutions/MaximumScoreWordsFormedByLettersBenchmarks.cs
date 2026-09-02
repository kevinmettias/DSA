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
    private const int AlphabetSize = 26;
    private const int RandomSeed = 4;
    private const int MaxLetterScoreExclusive = 10;
    private const int MinWordLength = 2;
    private const int MaxWordLengthExclusive = 5;
    private const int LetterBudgetDivisor = 2;

    [Params(8, 14)]
    public int WordCount;

    private string[] _words = null!;
    private char[] _letters = null!;
    private int[] _score = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _score = BuildScores(random);

        var totalUsage = new int[AlphabetSize];
        _words = BuildWords(random, totalUsage);
        _letters = BuildLetterPool(totalUsage);
    }

    private static int[] BuildScores(Random random)
    {
        var score = new int[AlphabetSize];

        foreach (var letter in Alphabet)
        {
            score[letter - 'a'] = random.Next(1, MaxLetterScoreExclusive);
        }

        return score;
    }

    private string[] BuildWords(Random random, int[] totalUsage)
    {
        var words = new string[WordCount];

        for (var i = 0; i < WordCount; i++)
        {
            words[i] = GenerateWord(random, totalUsage);
        }

        return words;
    }

    private static char[] BuildLetterPool(int[] totalUsage)
    {
        var letters = new List<char>();

        for (var c = 0; c < AlphabetSize; c++)
        {
            for (var n = 0; n < (totalUsage[c] + 1) / LetterBudgetDivisor; n++)
            {
                letters.Add((char)('a' + c));
            }
        }

        return letters.ToArray();
    }

    private static string GenerateWord(Random random, int[] totalUsage)
    {
        var length = random.Next(MinWordLength, MaxWordLengthExclusive);
        var chars = new char[length];

        for (var j = 0; j < length; j++)
        {
            chars[j] = Alphabet[random.Next(Alphabet.Length)];
            totalUsage[chars[j] - 'a']++;
        }

        return new string(chars);
    }

    [Benchmark(Baseline = true)]
    public int NaiveRecursion()
    {
        var available = LetterCounts(_letters);
        var wordCounts = _words.Select(LetterCounts).ToArray();
        var wordScores = _words.Select(WordScore).ToArray();

        return Search(0, available, 0, new WordData(wordCounts, wordScores));
    }

    private int Search(int index, int[] remaining, int currentScore, WordData words)
    {
        if (index == _words.Length)
        {
            return currentScore;
        }

        var skipped = Search(index + 1, remaining, currentScore, words);
        var counts = words.Counts[index];

        if (!Fits(counts, remaining))
        {
            return skipped;
        }

        ApplyCounts(remaining, counts, subtract: true);
        var included = Search(index + 1, remaining, currentScore + words.Scores[index], words);
        ApplyCounts(remaining, counts, subtract: false);

        return Math.Max(skipped, included);
    }

    private static bool Fits(int[] counts, int[] remaining)
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

    private static void ApplyCounts(int[] remaining, int[] counts, bool subtract)
    {
        var sign = subtract ? -1 : 1;

        for (var c = 0; c < AlphabetSize; c++)
        {
            remaining[c] += sign * counts[c];
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
        var counts = new int[AlphabetSize];

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

    private readonly record struct WordData(int[][] Counts, int[] Scores);

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
                var counts = new int[AlphabetSize];
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

                for (var c = 0; c < AlphabetSize; c++)
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

                for (var c = 0; c < AlphabetSize; c++)
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

                for (var c = 0; c < AlphabetSize; c++)
                {
                    _available[c] += counts[c];
                }

                CurrentScore -= _wordScores[Index];
            }
        }
    }
}
