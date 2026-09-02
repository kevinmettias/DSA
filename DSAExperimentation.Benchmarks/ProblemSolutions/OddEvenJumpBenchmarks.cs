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
    // LC problem number, used as the deterministic setup seed.
    private const int RandomSeed = 975;
    // Backward fill starts one index before the last, since the last index
    // is already seeded as the base case.
    private const int BackwardScanStartOffset = 2;

    private readonly record struct JumpTargets(int[] OddNext, int[] EvenNext);
    private readonly record struct Reachability(bool[] Odd, bool[] Even);

    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(1, Length)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceScan()
    {
        var oddNext = BruteNext(_values, ascending: true);
        var evenNext = BruteNext(_values, ascending: false);

        return CountGoodStarts(oddNext, evenNext);
    }

    [Benchmark]
    public int MergeSortStackSweep()
    {
        var oddNext = SortedNext(_values, ascending: true);
        var evenNext = SortedNext(_values, ascending: false);

        return CountGoodStarts(oddNext, evenNext);
    }

    private static int CountGoodStarts(int[] oddNext, int[] evenNext)
    {
        var targets = new JumpTargets(oddNext, evenNext);
        var reachability = InitializeReachability(oddNext.Length);

        FillReachability(targets, reachability);

        return CountTrue(reachability.Odd);
    }

    private static Reachability InitializeReachability(int n)
    {
        var odd = new bool[n];
        var even = new bool[n];
        odd[n - 1] = even[n - 1] = true;

        return new Reachability(odd, even);
    }

    private static void FillReachability(JumpTargets targets, Reachability reachability)
    {
        for (var i = targets.OddNext.Length - BackwardScanStartOffset; i >= 0; i--)
        {
            if (targets.OddNext[i] != -1)
            {
                reachability.Odd[i] = reachability.Even[targets.OddNext[i]];
            }

            if (targets.EvenNext[i] != -1)
            {
                reachability.Even[i] = reachability.Odd[targets.EvenNext[i]];
            }
        }
    }

    private static int CountTrue(bool[] values)
    {
        var count = 0;

        foreach (var value in values)
        {
            if (value)
            {
                count++;
            }
        }

        return count;
    }

    // For each index, scans every later index directly to find the smallest
    // qualifying value (odd) or largest qualifying value (even), ties broken toward
    // the nearer index - the O(n^2) baseline the sort-plus-stack sweep below replaces.
    private static int[] BruteNext(int[] arr, bool ascending)
    {
        var next = InitializeNext(arr.Length);

        for (var i = 0; i < arr.Length; i++)
        {
            next[i] = FindNextIndex(arr, i, ascending);
        }

        return next;
    }

    private static int[] InitializeNext(int n)
    {
        var next = new int[n];
        Array.Fill(next, -1);

        return next;
    }

    private static int FindNextIndex(int[] arr, int i, bool ascending)
    {
        var best = -1;

        for (var j = i + 1; j < arr.Length; j++)
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

        return best;
    }

    private static int[] SortedNext(int[] arr, bool ascending)
    {
        var indices = BuildSortedIndices(arr, ascending);
        var next = InitializeNext(arr.Length);

        FillNextViaStackSweep(indices, next);

        return next;
    }

    private static int[] BuildSortedIndices(int[] arr, bool ascending)
    {
        var indices = Enumerable.Range(0, arr.Length).ToArray();
        var comparer = BuildIndexComparer(arr, ascending);

        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(indices), comparer);

        return indices;
    }

    private static Comparer<int> BuildIndexComparer(int[] arr, bool ascending)
    {
        return ascending
            ? Comparer<int>.Create((a, b) => arr[a] != arr[b] ? arr[a].CompareTo(arr[b]) : a.CompareTo(b))
            : Comparer<int>.Create((a, b) => arr[a] != arr[b] ? arr[b].CompareTo(arr[a]) : a.CompareTo(b));
    }

    private static void FillNextViaStackSweep(int[] indices, int[] next)
    {
        var pending = new JumpIndexStack();

        foreach (var i in indices)
        {
            PopSatisfied(pending, next, i);

            pending.Push(i);
        }
    }

    private static void PopSatisfied(JumpIndexStack pending, int[] next, int currentIndex)
    {
        while (pending.TryPeek(out var left) && left < currentIndex)
        {
            pending.TryPop(out _);
            next[left] = currentIndex;
        }
    }
}
