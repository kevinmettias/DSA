using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Fruit Into Baskets (LC 904): the O(n^2) per-start rescan (extend right until a
// third distinct type appears) vs. the O(n) grow/shrink sliding window that tracks
// type -> count-in-window in this repo's own HashMap<int,int>, whose own Count is
// the window's distinct-type count. _fruits alternates between only 2 tree types so
// the window (and BruteForce's inner scan) never needs to shrink for a third type -
// forcing BOTH strategies through their full-length scan instead of BruteForce
// breaking out after only 3 elements every time, the same "force the real worst
// case" convention TwoSumBenchmarks/SubarrayProductLessThanKBenchmarks establish.
[MemoryDiagnoser]
public class FruitIntoBasketsBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private int[] _fruits = null!;

    [GlobalSetup]
    public void Setup() => _fruits = Enumerable.Range(0, Length).Select(i => i % 2).ToArray();

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        var longest = 0;

        for (var start = 0; start < _fruits.Length; start++)
        {
            var seen = new HashSet<int>();

            for (var end = start; end < _fruits.Length; end++)
            {
                seen.Add(_fruits[end]);

                if (seen.Count > 2)
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
        var basketCounts = new HashMap<int, int>();
        var windowStart = 0;
        var longest = 0;

        for (var windowEnd = 0; windowEnd < _fruits.Length; windowEnd++)
        {
            basketCounts.TryGetValue(_fruits[windowEnd], out var count);
            basketCounts.Set(_fruits[windowEnd], count + 1);

            while (basketCounts.Count > 2)
            {
                var leaving = _fruits[windowStart];
                basketCounts.TryGetValue(leaving, out var leavingCount);

                if (leavingCount == 1)
                {
                    basketCounts.TryRemove(leaving);
                }
                else
                {
                    basketCounts.Set(leaving, leavingCount - 1);
                }

                windowStart++;
            }

            longest = Math.Max(longest, windowEnd - windowStart + 1);
        }

        return longest;
    }
}
