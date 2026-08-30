using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Longest Substring with At Least K Repeating Characters (LC 395): the O(n^2)
// brute-force scan over every substring (a plain BCL Dictionary tracking how many
// distinct characters in the growing window are still below k) vs. the O(n)
// expected-case divide-and-conquer over this repo's own HashMap<char,int> - same
// frequency-count primitive MinimumWindowSubstringBenchmarks already uses -
// splitting the range at any character whose total count falls below k and
// recursing on both halves.
[MemoryDiagnoser]
public class LongestSubstringWithAtLeastKRepeatingCharactersBenchmarks
{
    private const int K = 3;

    [Params(200, 5_000)]
    public int Length;

    private string _s = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _s = new string(Enumerable.Range(0, Length).Select(_ => (char)('a' + random.Next(0, 4))).ToArray());
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        var best = 0;

        for (var start = 0; start < _s.Length; start++)
        {
            var counts = new Dictionary<char, int>();
            var belowK = 0;

            for (var end = start; end < _s.Length; end++)
            {
                var ch = _s[end];
                var newCount = counts.GetValueOrDefault(ch) + 1;
                counts[ch] = newCount;

                if (newCount == 1)
                {
                    belowK++;
                }
                else if (newCount == K)
                {
                    belowK--;
                }

                if (belowK == 0)
                {
                    best = Math.Max(best, end - start + 1);
                }
            }
        }

        return best;
    }

    [Benchmark]
    public int DivideAndConquerHashMap() => LongestSubstringInRange(_s, 0, _s.Length);

    private static int LongestSubstringInRange(string s, int start, int end)
    {
        if (end - start < K)
        {
            return 0;
        }

        var counts = new HashMap<char, int>();

        for (var i = start; i < end; i++)
        {
            counts.TryGetValue(s[i], out var count);
            counts.Set(s[i], count + 1);
        }

        for (var i = start; i < end; i++)
        {
            counts.TryGetValue(s[i], out var count);

            if (count < K)
            {
                var left = LongestSubstringInRange(s, start, i);
                var right = LongestSubstringInRange(s, i + 1, end);
                return Math.Max(left, right);
            }
        }

        return end - start;
    }
}
