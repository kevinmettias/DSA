using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Shortest Unsorted Continuous Subarray (LC 581): O(n^2) selection sort vs. this
// repo's own O(n log n) MergeSort over ArrayIndexedSequence - the same
// MaximumGapBenchmarks shape - each followed by the same left/right scan
// comparing the sorted copy against the original to find the span that needs
// re-sorting. _values is random, so a genuinely already-sorted array is
// astronomically unlikely and both strategies do real work.
[MemoryDiagnoser]
public class ShortestUnsortedContinuousSubarrayBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(581);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(0, 1_000_000)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int SelectionSortScan()
    {
        var sorted = _values.ToArray();

        for (var i = 0; i < sorted.Length - 1; i++)
        {
            var minIndex = i;
            for (var j = i + 1; j < sorted.Length; j++)
            {
                if (sorted[j] < sorted[minIndex])
                {
                    minIndex = j;
                }
            }

            (sorted[i], sorted[minIndex]) = (sorted[minIndex], sorted[i]);
        }

        return UnsortedSpanLength(sorted);
    }

    [Benchmark]
    public int MergeSortScan()
    {
        var sorted = _values.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));
        return UnsortedSpanLength(sorted);
    }

    private int UnsortedSpanLength(int[] sorted)
    {
        var left = 0;
        while (left < _values.Length && _values[left] == sorted[left])
        {
            left++;
        }

        if (left == _values.Length)
        {
            return 0;
        }

        var right = _values.Length - 1;
        while (_values[right] == sorted[right])
        {
            right--;
        }

        return right - left + 1;
    }
}
