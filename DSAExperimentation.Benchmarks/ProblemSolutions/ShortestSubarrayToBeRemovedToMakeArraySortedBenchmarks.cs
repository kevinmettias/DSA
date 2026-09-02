using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Shortest Subarray to be Removed to Make Array Sorted (LC 1574): the textbook
// cubic brute force - every (start, end) removal range checked by re-scanning the
// merged prefix+suffix for sortedness (the same O(n^3) "every range, full rescan"
// shape ThreeSumBenchmarks' own cubic baseline uses) - vs. the O(n) two-pointer
// prefix/suffix bounds plus this repo's own BinarySearch.LowerBound over an
// ArraySequence<int> witness (ShortestSubarrayToBeRemovedToMakeArraySortedTests
// precedent), O(n log n) overall.
[MemoryDiagnoser]
public class ShortestSubarrayToBeRemovedToMakeArraySortedBenchmarks
{
    private const int RandomSeed = 1574;
    private const int MaxElementValue = 1_000;

    [Params(80, 300)]
    public int Length;

    private int[] _arr = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _arr = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxElementValue)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        var n = _arr.Length;
        var best = n;

        for (var start = 0; start <= n; start++)
        {
            for (var end = start; end <= n; end++)
            {
                if (IsMergedSorted(_arr, start, end))
                {
                    best = Math.Min(best, end - start);
                }
            }
        }

        return best;
    }

    private static bool IsMergedSorted(int[] arr, int removeStart, int removeEnd)
    {
        var previous = int.MinValue;

        if (!IsNonDecreasingFrom(arr, 0, removeStart, ref previous))
        {
            return false;
        }

        return IsNonDecreasingFrom(arr, removeEnd, arr.Length, ref previous);
    }

    private static bool IsNonDecreasingFrom(int[] arr, int start, int end, ref int previous)
    {
        for (var i = start; i < end; i++)
        {
            if (arr[i] < previous)
            {
                return false;
            }

            previous = arr[i];
        }

        return true;
    }

    [Benchmark]
    public int TwoPointerBinarySearch()
    {
        var arr = _arr;
        var n = arr.Length;

        var left = FindPrefixEnd(arr, n);

        if (left == n - 1)
        {
            return 0;
        }

        var right = FindSuffixStart(arr, n);
        var best = Math.Min(n - left - 1, right);

        return BestOverPrefix(arr, left, right, best);
    }

    private static int FindPrefixEnd(int[] arr, int n)
    {
        var left = 0;

        while (left + 1 < n && arr[left] <= arr[left + 1])
        {
            left++;
        }

        return left;
    }

    private static int FindSuffixStart(int[] arr, int n)
    {
        var right = n - 1;

        while (right > 0 && arr[right - 1] <= arr[right])
        {
            right--;
        }

        return right;
    }

    private static int BestOverPrefix(int[] arr, int left, int right, int best)
    {
        var suffix = new ArraySequence<int>(arr[right..]);

        for (var i = 0; i <= left; i++)
        {
            var j = right + BinarySearch.LowerBound(suffix, arr[i]);
            best = Math.Min(best, j - i - 1);
        }

        return best;
    }
}
