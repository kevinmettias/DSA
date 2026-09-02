using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Number of Valid Words for Each Puzzle (LC 1178): the O(PuzzleCount * WordCount)
// direct-comparison brute force vs. grouping words into this repo's own
// HashMap<int,int> by their 26-bit letter mask, then answering each puzzle with an
// O(2^6) submask enumeration over that map - O(WordCount + PuzzleCount * 64) total.
[MemoryDiagnoser]
public class NumberOfValidWordsForEachPuzzleBenchmarks
{
    private const int PuzzleCount = 50;
    private const int MinWordLength = 3;
    private const int WordLengthUpperBound = 9; // exclusive; word length ranges [3, 8]
    private const int PuzzleLength = 7; // LC 1178: every puzzle has exactly 7 distinct letters
    private const int AlphabetSize = 26;

    [Params(200, 4_000)]
    public int WordCount;

    private string[] _words = null!;
    private string[] _puzzles = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _words = Enumerable.Range(0, WordCount).Select(_ => RandomWord(random)).ToArray();
        _puzzles = Enumerable.Range(0, PuzzleCount).Select(_ => RandomPuzzle(random)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        var total = 0;

        foreach (var puzzle in _puzzles)
        {
            var puzzleMask = LetterMask(puzzle);
            var firstLetterBit = 1 << (puzzle[0] - 'a');

            foreach (var word in _words)
            {
                var wordMask = LetterMask(word);

                if ((wordMask & firstLetterBit) != 0 && (wordMask & puzzleMask) == wordMask)
                {
                    total++;
                }
            }
        }

        return total;
    }

    [Benchmark]
    public int HashMapSubsetEnumeration()
    {
        var wordCountsByMask = new HashMap<int, int>();
        foreach (var word in _words)
        {
            var mask = LetterMask(word);
            wordCountsByMask.TryGetValue(mask, out var existing);
            wordCountsByMask.Set(mask, existing + 1);
        }

        var total = 0;
        foreach (var puzzle in _puzzles)
        {
            total += CountValidWords(wordCountsByMask, LetterMask(puzzle), 1 << (puzzle[0] - 'a'));
        }

        return total;
    }

    private static int CountValidWords(HashMap<int, int> wordCountsByMask, int puzzleMask, int firstLetterBit)
    {
        var total = 0;
        var submask = puzzleMask;

        while (true)
        {
            if ((submask & firstLetterBit) != 0 && wordCountsByMask.TryGetValue(submask, out var wordCount))
            {
                total += wordCount;
            }

            if (submask == 0)
            {
                break;
            }

            submask = (submask - 1) & puzzleMask;
        }

        return total;
    }

    private static int LetterMask(string value)
    {
        var mask = 0;
        foreach (var ch in value)
        {
            mask |= 1 << (ch - 'a');
        }

        return mask;
    }

    private static string RandomWord(Random random)
    {
        var length = random.Next(MinWordLength, WordLengthUpperBound);
        var letters = new HashSet<char>();
        while (letters.Count < length)
        {
            letters.Add((char)('a' + random.Next(0, AlphabetSize)));
        }

        return new string(letters.ToArray());
    }

    private static string RandomPuzzle(Random random)
    {
        var letters = new HashSet<char>();
        while (letters.Count < PuzzleLength)
        {
            letters.Add((char)('a' + random.Next(0, AlphabetSize)));
        }

        return new string(letters.ToArray());
    }
}
