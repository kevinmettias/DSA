using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Array Partition (LC 561): repeatedly scanning for the two smallest remaining
// elements (O(n^2), no sort) vs. this repo's own MergeSort over
// ArrayIndexedSequence (O(n log n)) followed by summing every even-indexed
// element - both compute the same maximized sum of pair-minimums.
[MemoryDiagnoser]
public class ArrayPartitionBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(561);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(-10_000, 10_000)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RepeatedSmallestPairScan()
    {
        var used = new bool[_values.Length];
        var sum = 0;

        for (var pair = 0; pair < _values.Length / 2; pair++)
        {
            var firstIndex = -1;
            var secondIndex = -1;

            for (var i = 0; i < _values.Length; i++)
            {
                if (used[i])
                {
                    continue;
                }

                if (firstIndex < 0 || _values[i] < _values[firstIndex])
                {
                    secondIndex = firstIndex;
                    firstIndex = i;
                }
                else if (secondIndex < 0 || _values[i] < _values[secondIndex])
                {
                    secondIndex = i;
                }
            }

            used[firstIndex] = true;
            used[secondIndex] = true;
            sum += _values[firstIndex];
        }

        return sum;
    }

    [Benchmark]
    public int MergeSortPairSum()
    {
        var sorted = _values.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var sum = 0;
        for (var i = 0; i < sorted.Length; i += 2)
        {
            sum += sorted[i];
        }

        return sum;
    }
}
