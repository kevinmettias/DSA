using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Longest Substring Without Repeating Characters (LC 3): the O(n^2) brute force
// re-scans forward from every start index until it hits a repeat, vs. the O(n)
// sliding window that tracks each character's last-seen index in this repo's own
// HashMap<char,int> and jumps the window's left edge straight past a repeat
// instead of re-scanning from the next start index. _text is deliberately built
// from all-distinct characters (no repeat anywhere) so BOTH strategies are forced
// through their full worst-case scan - a small, repeat-heavy alphabet would let
// BruteForce's inner loop break out after only a handful of characters every
// time (pigeonhole caps any repeat-free run at the alphabet size), making it look
// artificially competitive instead of exposing its real O(n^2) cost.
[MemoryDiagnoser]
public class LongestSubstringWithoutRepeatingCharactersBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private string _text = null!;

    [GlobalSetup]
    public void Setup()
    {
        _text = new string(Enumerable.Range(0, Length).Select(i => (char)(256 + i)).ToArray());
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        var longest = 0;

        for (var start = 0; start < _text.Length; start++)
        {
            var seen = new HashSet<char>();

            for (var end = start; end < _text.Length; end++)
            {
                if (!seen.Add(_text[end]))
                {
                    break;
                }

                longest = Math.Max(longest, end - start + 1);
            }
        }

        return longest;
    }

    [Benchmark]
    public int SlidingWindowHashMap()
    {
        var lastSeenIndex = new HashMap<char, int>();
        var windowStart = 0;
        var longest = 0;

        for (var windowEnd = 0; windowEnd < _text.Length; windowEnd++)
        {
            var current = _text[windowEnd];

            if (lastSeenIndex.TryGetValue(current, out var previousIndex) && previousIndex >= windowStart)
            {
                windowStart = previousIndex + 1;
            }

            lastSeenIndex.Set(current, windowEnd);
            longest = Math.Max(longest, windowEnd - windowStart + 1);
        }

        return longest;
    }
}
