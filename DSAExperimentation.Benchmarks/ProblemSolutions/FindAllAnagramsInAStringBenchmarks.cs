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

    // LeetCode problem number, reused as the RNG seed for reproducible benchmark input.
    private const int RandomSeed = 438;

    [Params(2_000, 20_000)]
    public int Length;

    private string _s = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
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
            var candidateWindow = _s.Substring(start, Pattern.Length);
            var window = BuildFrequencyMap(candidateWindow);
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
            bool isMatch;
            (matched, isMatch) = AdvanceWindow(i, need, window, matched);

            if (isMatch)
            {
                matches++;
            }
        }

        return matches;
    }

    private (int Matched, bool IsMatch) AdvanceWindow(int i, HashMap<char, int> need, HashMap<char, int> window, int matched)
    {
        matched = AdvanceEnteringChar(_s[i], need, window, matched);

        if (i < Pattern.Length - 1)
        {
            return (matched, false);
        }

        var isMatch = matched == need.Count;

        var leaving = _s[i - Pattern.Length + 1];
        matched = AdvanceLeavingChar(leaving, need, window, matched);

        return (matched, isMatch);
    }

    private static int AdvanceEnteringChar(char entering, HashMap<char, int> need, HashMap<char, int> window, int matched)
    {
        if (!need.TryGetValue(entering, out var neededCount))
        {
            return matched;
        }

        window.TryGetValue(entering, out var count);
        window.Set(entering, count + 1);

        return count + 1 == neededCount ? matched + 1 : matched;
    }

    private static int AdvanceLeavingChar(char leaving, HashMap<char, int> need, HashMap<char, int> window, int matched)
    {
        if (!need.TryGetValue(leaving, out var neededLeaving))
        {
            return matched;
        }

        window.TryGetValue(leaving, out var leavingCount);
        var updatedMatched = leavingCount == neededLeaving ? matched - 1 : matched;
        window.Set(leaving, leavingCount - 1);
        return updatedMatched;
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
