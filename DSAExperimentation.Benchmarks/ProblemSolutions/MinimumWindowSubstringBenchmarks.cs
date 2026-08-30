using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Window Substring (LC 76): the brute force restarts a fresh Dictionary
// scan from every start index, vs. the O(|s| + |t|) sliding window that tracks
// remaining need in this repo's own HashMap<char,int> and only ever moves each
// pointer forward. _t is deliberately built from characters absent from _s (same
// "force the unreachable worst case" trick TwoSumBenchmarks/LongestSubstring...
// Benchmarks already use) so neither strategy ever satisfies "missing == 0" and
// early-exits - BruteForce is forced through every O(n^2) start/end pair instead of
// breaking out after a handful of characters, and SlidingWindowHashMap is forced
// through its full single O(n) pass with the left pointer never advancing.
[MemoryDiagnoser]
public class MinimumWindowSubstringBenchmarks
{
    private const string Target = "XYZ";

    [Params(200, 5_000)]
    public int Length;

    private string _s = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _s = new string(Enumerable.Range(0, Length).Select(_ => (char)('a' + random.Next(0, 26))).ToArray());
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        var bestLength = int.MaxValue;

        for (var start = 0; start < _s.Length; start++)
        {
            var need = new Dictionary<char, int>();

            foreach (var ch in Target)
            {
                need[ch] = need.GetValueOrDefault(ch) + 1;
            }

            var missing = Target.Length;

            for (var end = start; end < _s.Length; end++)
            {
                var incoming = _s[end];

                if (need.TryGetValue(incoming, out var remaining))
                {
                    need[incoming] = remaining - 1;

                    if (remaining > 0)
                    {
                        missing--;
                    }
                }

                if (missing == 0)
                {
                    bestLength = Math.Min(bestLength, end - start + 1);
                    break;
                }
            }
        }

        return bestLength;
    }

    [Benchmark]
    public int SlidingWindowHashMap()
    {
        var need = new HashMap<char, int>();

        foreach (var ch in Target)
        {
            need.TryGetValue(ch, out var count);
            need.Set(ch, count + 1);
        }

        var missing = Target.Length;
        var left = 0;
        var bestLength = int.MaxValue;

        for (var right = 0; right < _s.Length; right++)
        {
            var incoming = _s[right];

            if (need.TryGetValue(incoming, out var remaining))
            {
                need.Set(incoming, remaining - 1);

                if (remaining > 0)
                {
                    missing--;
                }
            }

            while (missing == 0)
            {
                bestLength = Math.Min(bestLength, right - left + 1);
                var outgoing = _s[left];

                if (need.TryGetValue(outgoing, out var freed))
                {
                    need.Set(outgoing, freed + 1);

                    if (freed >= 0)
                    {
                        missing++;
                    }
                }

                left++;
            }
        }

        return bestLength;
    }
}
