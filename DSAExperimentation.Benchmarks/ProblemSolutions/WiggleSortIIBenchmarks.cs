using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Wiggle Sort II (LC 324): the textbook O(n^2) selection sort vs. this repo's own
// O(n log n) MergeSort over ArrayIndexedSequence (HIndexBenchmarks precedent),
// each followed by the identical O(n) reversed-half interleave into even/odd index
// positions - the sort is the only part that varies between the two benchmarks.
[MemoryDiagnoser]
public class WiggleSortIIBenchmarks
{
    [Params(200, 2_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(324);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(0, Length / 2)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] SelectionSortInterleave()
    {
        var sorted = (int[])_values.Clone();

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

        return Interleave(sorted);
    }

    [Benchmark]
    public int[] MergeSortInterleave()
    {
        var sorted = (int[])_values.Clone();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        return Interleave(sorted);
    }

    private static int[] Interleave(int[] sorted)
    {
        var n = sorted.Length;
        var result = new int[n];
        var lowIndex = (n - 1) / 2;
        var highIndex = n - 1;

        for (var i = 0; i < n; i++)
        {
            result[i] = i % 2 == 0 ? sorted[lowIndex--] : sorted[highIndex--];
        }

        return result;
    }
}
