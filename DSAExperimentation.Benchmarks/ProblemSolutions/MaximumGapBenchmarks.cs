using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Gap (LC 164): O(n^2) selection sort vs. this repo's own O(n log n)
// MergeSort over ArrayIndexedSequence, each followed by the same linear scan
// for the largest gap between sorted neighbors.
[MemoryDiagnoser]
public class MaximumGapBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(164);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(0, 1_000_000)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int SelectionSort()
    {
        var arr = _values.ToArray();

        for (var i = 0; i < arr.Length - 1; i++)
        {
            var minIndex = i;
            for (var j = i + 1; j < arr.Length; j++)
            {
                if (arr[j] < arr[minIndex])
                {
                    minIndex = j;
                }
            }

            (arr[i], arr[minIndex]) = (arr[minIndex], arr[i]);
        }

        return MaxAdjacentGap(arr);
    }

    [Benchmark]
    public int MergeSortScan()
    {
        var arr = _values.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(arr));
        return MaxAdjacentGap(arr);
    }

    private static int MaxAdjacentGap(int[] sorted)
    {
        var maxGap = 0;
        for (var i = 1; i < sorted.Length; i++)
        {
            maxGap = Math.Max(maxGap, sorted[i] - sorted[i - 1]);
        }

        return maxGap;
    }
}
