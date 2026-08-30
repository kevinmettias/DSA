using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;
using JumpIndexStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Odd Even Jump (LC 975): the quadratic per-index forward scan for each jump's
// target vs. this repo's own MergeSort.Sort<int,ArrayIndexedSequence<int>> ordering
// indices by (value, index) plus a single monotonic Stack<int> sweep over that order
// (DailyTemperaturesBenchmarks precedent) - O(n log n) instead of O(n^2), for both
// the odd (>=) and even (<=) jump directions.
[MemoryDiagnoser]
public class OddEvenJumpBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(975);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(1, Length)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceScan() => CountGoodStarts(BruteNext(_values, ascending: true), BruteNext(_values, ascending: false));

    [Benchmark]
    public int MergeSortStackSweep() => CountGoodStarts(SortedNext(_values, ascending: true), SortedNext(_values, ascending: false));

    private static int CountGoodStarts(int[] oddNext, int[] evenNext)
    {
        var n = oddNext.Length;
        var odd = new bool[n];
        var even = new bool[n];
        odd[n - 1] = even[n - 1] = true;

        var goodStarts = 1;

        for (var i = n - 2; i >= 0; i--)
        {
            if (oddNext[i] != -1)
            {
                odd[i] = even[oddNext[i]];
            }

            if (evenNext[i] != -1)
            {
                even[i] = odd[evenNext[i]];
            }

            if (odd[i])
            {
                goodStarts++;
            }
        }

        return goodStarts;
    }

    // For each index, scans every later index directly to find the smallest
    // qualifying value (odd) or largest qualifying value (even), ties broken toward
    // the nearer index - the O(n^2) baseline the sort-plus-stack sweep below replaces.
    private static int[] BruteNext(int[] arr, bool ascending)
    {
        var n = arr.Length;
        var next = new int[n];
        Array.Fill(next, -1);

        for (var i = 0; i < n; i++)
        {
            var best = -1;

            for (var j = i + 1; j < n; j++)
            {
                var qualifies = ascending ? arr[j] >= arr[i] : arr[j] <= arr[i];

                if (!qualifies)
                {
                    continue;
                }

                if (best == -1 || (ascending ? arr[j] < arr[best] : arr[j] > arr[best]))
                {
                    best = j;
                }
            }

            next[i] = best;
        }

        return next;
    }

    private static int[] SortedNext(int[] arr, bool ascending)
    {
        var n = arr.Length;
        var indices = Enumerable.Range(0, n).ToArray();

        var comparer = ascending
            ? Comparer<int>.Create((a, b) => arr[a] != arr[b] ? arr[a].CompareTo(arr[b]) : a.CompareTo(b))
            : Comparer<int>.Create((a, b) => arr[a] != arr[b] ? arr[b].CompareTo(arr[a]) : a.CompareTo(b));

        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(indices), comparer);

        var next = new int[n];
        Array.Fill(next, -1);

        var pending = new JumpIndexStack();

        foreach (var i in indices)
        {
            while (pending.TryPeek(out var left) && left < i)
            {
                pending.TryPop(out _);
                next[left] = i;
            }

            pending.Push(i);
        }

        return next;
    }
}
