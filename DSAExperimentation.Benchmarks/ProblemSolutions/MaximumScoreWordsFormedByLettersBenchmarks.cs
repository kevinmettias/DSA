using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumScoreWordsFormedByLetters;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumScoreWordsFormedByLettersSolution's, the same
// methods MaximumScoreWordsFormedByLettersTests proves correct. A hand-specialized
// include/skip recursion over the word list vs. this repo's own Backtrack.Search
// closed over the identical choose/explore/unchoose steps - the same comparison
// PartitionToKEqualSumSubsetsBenchmarks already makes for LC 698, here choosing
// "use this word" vs. "skip it" per index instead of "which bucket does this number
// join."
//
// The letter budget is set to half of what every word combined would need, so the
// fit check genuinely fails on some branches instead of every word always fitting -
// a real knapsack-shaped search, not just "sum everything."
//
// LeetCode's own input shape - the word list, the letter pool and the 26-entry
// score vector - is already what both strategies take, so [GlobalSetup] only
// decides how large the workload is and hands the finished input straight over;
// there is no construction left for a hoisted overload to lift out of the measured
// methods.
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

    private string[] _words = [];

    private char[] _letters = [];
    private int[] _score = [];
    [Params(8, 14)]
    public int WordCount { get; set; }

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

    [Benchmark(Baseline = true)]
    public int NaiveRecursion() =>
        MaximumScoreWordsFormedByLettersSolution.MaxScoreWordsByNaiveRecursion(_words, _letters, _score);

    [Benchmark]
    public int BacktrackPrimitive() =>
        MaximumScoreWordsFormedByLettersSolution.MaxScoreWordsByBacktrackSearch(_words, _letters, _score);
}
