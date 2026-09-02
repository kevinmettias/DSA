using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Sort an Array (LC 912): the textbook O(n^2) insertion sort vs. this repo's own
// MergeSort over ArrayIndexedSequence (O(n log n)). Both sort a fresh copy of the
// same randomized input each invocation so neither benefits from the other's
// partially-sorted leftovers.
[MemoryDiagnoser]
public class SortAnArrayBenchmarks
{
    // LC problem number, reused as the Random seed for reproducible benchmark input.
    private const int RandomSeed = 912;

    private const int ValueRange = 50_000;

    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(-ValueRange, ValueRange)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] InsertionSort()
    {
        var sorted = _values.ToArray();

        for (var i = 1; i < sorted.Length; i++)
        {
            var current = sorted[i];
            var j = i - 1;

            while (j >= 0 && sorted[j] > current)
            {
                sorted[j + 1] = sorted[j];
                j--;
            }

            sorted[j + 1] = current;
        }

        return sorted;
    }

    [Benchmark]
    public int[] MergeSortAscending()
    {
        var sorted = _values.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));
        return sorted;
    }
}
