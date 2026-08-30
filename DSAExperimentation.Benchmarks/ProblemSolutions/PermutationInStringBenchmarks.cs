using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Permutation in String (LC 567): rebuilding and comparing a fresh frequency
// HashMap<char,int> for every window start (O(n*m)) vs. a single sliding pass
// that maintains one window HashMap<char,int> incrementally, using a running
// "matched distinct characters" counter instead of a full per-window comparison
// (O(n+m)) - the same shape this repo's Find All Anagrams in a String (438)
// benchmark already uses, here returning on the first match instead of
// collecting every one. s1 is deliberately absent from s2 so both strategies are
// forced through their full worst-case scan.
[MemoryDiagnoser]
public class PermutationInStringBenchmarks
{
    private const string Pattern = "aeiou";

    [Params(2_000, 20_000)]
    public int Length;

    private string _s2 = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(567);
        const string alphabet = "bcdfghjklmnpqrstvwxyz";
        var chars = new char[Length];
        for (var i = 0; i < Length; i++)
        {
            chars[i] = alphabet[random.Next(alphabet.Length)];
        }

        _s2 = new string(chars);
    }

    [Benchmark(Baseline = true)]
    public bool PerWindowFrequencyRebuild()
    {
        var target = BuildFrequencyMap(Pattern);

        for (var start = 0; start <= _s2.Length - Pattern.Length; start++)
        {
            var window = BuildFrequencyMap(_s2.Substring(start, Pattern.Length));
            if (FrequenciesEqual(window, target))
            {
                return true;
            }
        }

        return false;
    }

    [Benchmark]
    public bool SlidingWindowFrequencyMap()
    {
        var need = BuildFrequencyMap(Pattern);
        var window = new HashMap<char, int>();
        var matched = 0;

        for (var i = 0; i < _s2.Length; i++)
        {
            if (need.TryGetValue(_s2[i], out var needed))
            {
                window.TryGetValue(_s2[i], out var count);
                window.Set(_s2[i], count + 1);
                if (count + 1 == needed)
                {
                    matched++;
                }
            }

            if (i < Pattern.Length - 1)
            {
                continue;
            }

            if (matched == need.Count)
            {
                return true;
            }

            var leaving = _s2[i - Pattern.Length + 1];
            if (need.TryGetValue(leaving, out var neededLeaving))
            {
                window.TryGetValue(leaving, out var leavingCount);
                if (leavingCount == neededLeaving)
                {
                    matched--;
                }

                window.Set(leaving, leavingCount - 1);
            }
        }

        return false;
    }

    private static HashMap<char, int> BuildFrequencyMap(string value)
    {
        var counts = new HashMap<char, int>();
        foreach (var c in value)
        {
            counts.TryGetValue(c, out var count);
            counts.Set(c, count + 1);
        }

        return counts;
    }

    private static bool FrequenciesEqual(HashMap<char, int> window, HashMap<char, int> target)
    {
        if (window.Count != target.Count)
        {
            return false;
        }

        foreach (var key in target.Keys)
        {
            target.TryGetValue(key, out var expected);
            if (!window.TryGetValue(key, out var actual) || actual != expected)
            {
                return false;
            }
        }

        return true;
    }
}
