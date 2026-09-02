using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Height Checker (LC 1051): the textbook O(n^2) insertion sort of a copy vs.
// this repo's own MergeSort over ArrayIndexedSequence (O(n log n)) - both
// compute the same "expected" non-decreasing order and count where it differs
// from the original.
[MemoryDiagnoser]
public class HeightCheckerBenchmarks
{
    // LeetCode problem number, reused as the RNG seed for reproducible benchmark input.
    private const int RandomSeed = 1051;

    // Exclusive upper bound for the random height range: heights are 1..100.
    private const int HeightUpperBoundExclusive = 101;

    [Params(200, 5_000)]
    public int Length;

    private int[] _heights = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _heights = Enumerable.Range(0, Length).Select(_ => random.Next(1, HeightUpperBoundExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int InsertionSort()
    {
        var expected = _heights.ToArray();

        for (var i = 1; i < expected.Length; i++)
        {
            InsertOne(expected, i);
        }

        return CountMismatches(expected);
    }

    [Benchmark]
    public int MergeSortComparison()
    {
        var expected = _heights.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(expected));

        return CountMismatches(expected);
    }

    private static void InsertOne(int[] expected, int i)
    {
        var current = expected[i];
        var j = i - 1;

        while (j >= 0 && expected[j] > current)
        {
            expected[j + 1] = expected[j];
            j--;
        }

        expected[j + 1] = current;
    }

    private int CountMismatches(int[] expected)
    {
        var mismatches = 0;

        for (var i = 0; i < _heights.Length; i++)
        {
            if (_heights[i] != expected[i])
            {
                mismatches++;
            }
        }

        return mismatches;
    }
}
