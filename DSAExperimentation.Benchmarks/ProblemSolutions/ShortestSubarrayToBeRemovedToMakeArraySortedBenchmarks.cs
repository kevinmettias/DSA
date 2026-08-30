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
    [Params(80, 300)]
    public int Length;

    private int[] _arr = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1574);
        _arr = Enumerable.Range(0, Length).Select(_ => random.Next(1, 1_000)).ToArray();
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

        for (var i = 0; i < removeStart; i++)
        {
            if (arr[i] < previous)
            {
                return false;
            }

            previous = arr[i];
        }

        for (var i = removeEnd; i < arr.Length; i++)
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

        var left = 0;
        while (left + 1 < n && arr[left] <= arr[left + 1])
        {
            left++;
        }

        if (left == n - 1)
        {
            return 0;
        }

        var right = n - 1;
        while (right > 0 && arr[right - 1] <= arr[right])
        {
            right--;
        }

        var best = Math.Min(n - left - 1, right);

        var suffix = new ArraySequence<int>(arr[right..]);
        for (var i = 0; i <= left; i++)
        {
            var j = right + BinarySearch.LowerBound(suffix, arr[i]);
            best = Math.Min(best, j - i - 1);
        }

        return best;
    }
}
