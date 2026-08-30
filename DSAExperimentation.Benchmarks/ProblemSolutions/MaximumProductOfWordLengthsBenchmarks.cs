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
    [Params(100, 800)]
    public int WordCount;

    private string[] _words = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(318);
        _words = Enumerable.Range(0, WordCount)
            .Select(i => RandomWord(random, i % 2 == 0 ? 'a' : 'n', length: random.Next(4, 11)))
            .ToArray();
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
        => new(Enumerable.Range(0, length).Select(_ => (char)(alphabetStart + random.Next(13))).ToArray());

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
            var mask = 0;
            foreach (var c in word)
            {
                mask |= 1 << (c - 'a');
            }

            if (maskToMaxLength.TryGetValue(mask, out var existingLength) && existingLength >= word.Length)
            {
                continue;
            }

            maskToMaxLength.Set(mask, word.Length);
        }

        var masks = maskToMaxLength.Keys.ToArray();
        var lengths = new int[masks.Length];
        for (var i = 0; i < masks.Length; i++)
        {
            maskToMaxLength.TryGetValue(masks[i], out lengths[i]);
        }

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
