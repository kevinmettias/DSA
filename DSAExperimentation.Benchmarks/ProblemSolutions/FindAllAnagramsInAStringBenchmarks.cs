using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Find All Anagrams in a String (LC 438): rebuilding and comparing a fresh frequency
// HashMap<char,int> for every window start (O(n*m)) vs. a single sliding pass that
// maintains one window HashMap<char,int> incrementally, using a running "matched
// distinct characters" counter instead of a full per-window comparison (O(n+m)).
[MemoryDiagnoser]
public class FindAllAnagramsInAStringBenchmarks
{
    private const string Pattern = "aeiou";

    [Params(2_000, 20_000)]
    public int Length;

    private string _s = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(438);
        const string alphabet = "abcdefghijklmnopqrstuvwxyz";
        var chars = new char[Length];
        for (var i = 0; i < Length; i++)
        {
            chars[i] = alphabet[random.Next(alphabet.Length)];
        }

        _s = new string(chars);
    }

    [Benchmark(Baseline = true)]
    public int PerWindowFrequencyRebuild()
    {
        var target = BuildFrequencyMap(Pattern);
        var matches = 0;

        for (var start = 0; start <= _s.Length - Pattern.Length; start++)
        {
            var window = BuildFrequencyMap(_s.Substring(start, Pattern.Length));
            if (FrequenciesEqual(window, target))
            {
                matches++;
            }
        }

        return matches;
    }

    [Benchmark]
    public int SlidingWindowFrequencyMap()
    {
        var need = BuildFrequencyMap(Pattern);
        var window = new HashMap<char, int>();
        var matched = 0;
        var matches = 0;

        for (var i = 0; i < _s.Length; i++)
        {
            if (need.TryGetValue(_s[i], out var needed))
            {
                window.TryGetValue(_s[i], out var count);
                window.Set(_s[i], count + 1);
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
                matches++;
            }

            var leaving = _s[i - Pattern.Length + 1];
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

        return matches;
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
