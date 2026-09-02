using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Product of Word Lengths (LC 318): an O(n^2 * L1*L2) pairwise
// character scan vs. collapsing every word to a 26-bit letter-presence mask
// and using this repo's own HashMap<int,int> to dedupe words sharing a mask
// down to the longest one before an O(1)-per-pair mask AND check - the same
// approach MaximumProductOfWordLengthsTests uses. Words are split into two
// disjoint-alphabet halves ('a'-'m' vs. 'n'-'z') so every cross-half pair is
// guaranteed to share no letter, forcing CharacterScan's inner double loop
// through its full unmatched worst case instead of exiting early on the
// first shared letter.
[MemoryDiagnoser]
public class MaximumProductOfWordLengthsBenchmarks
{
    // LC problem number, reused as the fixed benchmark-data seed.
    private const int RandomSeed = 318;

    // Splits generated words between the 'a'-'m' and 'n'-'z' alphabet halves.
    private const int AlphabetHalfDivisor = 2;

    private const int MinWordLength = 4;
    private const int MaxWordLengthExclusive = 11;

    // 'a'-'m' and 'n'-'z' are each 13 letters wide (half of the 26-letter alphabet).
    private const int AlphabetHalfSize = 13;

    [Params(100, 800)]
    public int WordCount;

    private string[] _words = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _words = Enumerable.Range(0, WordCount).Select(i => NextRandomWord(random, i)).ToArray();
    }

    private static string NextRandomWord(Random random, int index)
    {
        var alphabetStart = index % AlphabetHalfDivisor == 0 ? 'a' : 'n';
        var length = random.Next(MinWordLength, MaxWordLengthExclusive);
        return RandomWord(random, alphabetStart, length);
    }

    [Benchmark(Baseline = true)]
    public int CharacterScan()
    {
        var best = 0;

        for (var i = 0; i < _words.Length; i++)
        {
            for (var j = i + 1; j < _words.Length; j++)
            {
                if (SharesLetter(_words[i], _words[j]))
                {
                    continue;
                }

                best = Math.Max(best, _words[i].Length * _words[j].Length);
            }
        }

        return best;
    }

    [Benchmark]
    public int BitmaskHashMap() => MaxProduct(_words);

    private static string RandomWord(Random random, char alphabetStart, int length)
        => new(Enumerable.Range(0, length).Select(_ => (char)(alphabetStart + random.Next(AlphabetHalfSize))).ToArray());

    private static bool SharesLetter(string a, string b)
    {
        foreach (var x in a)
        {
            foreach (var y in b)
            {
                if (x == y)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static int MaxProduct(string[] words)
    {
        var maskToMaxLength = new HashMap<int, int>();

        foreach (var word in words)
        {
            RecordLongestForMask(maskToMaxLength, word);
        }

        var masks = maskToMaxLength.Keys.ToArray();
        var lengths = new int[masks.Length];
        for (var i = 0; i < masks.Length; i++)
        {
            maskToMaxLength.TryGetValue(masks[i], out lengths[i]);
        }

        return BestDisjointPairProduct(masks, lengths);
    }

    private static void RecordLongestForMask(HashMap<int, int> maskToMaxLength, string word)
    {
        var mask = 0;
        foreach (var c in word)
        {
            mask |= 1 << (c - 'a');
        }

        if (maskToMaxLength.TryGetValue(mask, out var existingLength) && existingLength >= word.Length)
        {
            return;
        }

        maskToMaxLength.Set(mask, word.Length);
    }

    private static int BestDisjointPairProduct(int[] masks, int[] lengths)
    {
        var best = 0;

        for (var i = 0; i < masks.Length; i++)
        {
            for (var j = i + 1; j < masks.Length; j++)
            {
                if ((masks[i] & masks[j]) == 0)
                {
                    best = Math.Max(best, lengths[i] * lengths[j]);
                }
            }
        }

        return best;
    }
}
