using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Swaps To Make Sequences Increasing (LC 801): the textbook flat
// keep[i]/swap[i] tabulation vs. this repo's own Memoizer closing over a two-state
// (index, wasSwapped) recurrence. Values at index i are always {2i, 2i+1} in random
// order, so both keep and swap stay valid transitions at every step regardless of
// prior choices - both strategies run their full real workload instead of one
// branch getting pruned away immediately.
[MemoryDiagnoser]
public class MinimumSwapsToMakeSequencesIncreasingBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private int[] _nums1 = null!;
    private int[] _nums2 = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(801);
        _nums1 = new int[Length];
        _nums2 = new int[Length];

        for (var i = 0; i < Length; i++)
        {
            var low = 2 * i;
            var high = 2 * i + 1;

            if (random.Next(2) == 0)
            {
                _nums1[i] = low;
                _nums2[i] = high;
            }
            else
            {
                _nums1[i] = high;
                _nums2[i] = low;
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int Tabulation()
    {
        var n = _nums1.Length;
        var keep = new int[n];
        var swap = new int[n];
        swap[0] = 1;

        for (var i = 1; i < n; i++)
        {
            keep[i] = int.MaxValue;
            swap[i] = int.MaxValue;

            if (_nums1[i] > _nums1[i - 1] && _nums2[i] > _nums2[i - 1])
            {
                keep[i] = Math.Min(keep[i], keep[i - 1]);
                swap[i] = Math.Min(swap[i], swap[i - 1] + 1);
            }

            if (_nums1[i] > _nums2[i - 1] && _nums2[i] > _nums1[i - 1])
            {
                keep[i] = Math.Min(keep[i], swap[i - 1]);
                swap[i] = Math.Min(swap[i], keep[i - 1] + 1);
            }
        }

        return Math.Min(keep[n - 1], swap[n - 1]);
    }

    [Benchmark]
    public int MemoizedTwoState()
    {
        var last = _nums1.Length - 1;

        return Math.Min(
            Memoizer.Memoize<(int Index, bool Swapped), int>((last, false), Cost),
            Memoizer.Memoize<(int Index, bool Swapped), int>((last, true), Cost));
    }

    private int Cost((int Index, bool Swapped) state, Func<(int Index, bool Swapped), int> cost)
    {
        var (i, swapped) = state;

        if (i == 0)
        {
            return swapped ? 1 : 0;
        }

        var curA = swapped ? _nums2[i] : _nums1[i];
        var curB = swapped ? _nums1[i] : _nums2[i];
        var best = int.MaxValue;

        if (curA > _nums1[i - 1] && curB > _nums2[i - 1])
        {
            best = Math.Min(best, cost((i - 1, false)));
        }

        if (curA > _nums2[i - 1] && curB > _nums1[i - 1])
        {
            best = Math.Min(best, cost((i - 1, true)));
        }

        return best + (swapped ? 1 : 0);
    }
}
