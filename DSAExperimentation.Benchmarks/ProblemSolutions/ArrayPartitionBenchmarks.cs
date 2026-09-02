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
    // LC problem number, reused as the deterministic random seed.
    private const int RandomSeed = 561;

    // Symmetric bound for the generated values' range: [-ValueBound, ValueBound).
    private const int ValueBound = 10_000;

    // Elements are partitioned into pairs of this size, both when counting how
    // many pairs to scan and when stepping across every even-indexed element
    // of the sorted array.
    private const int PairSize = 2;

    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(-ValueBound, ValueBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RepeatedSmallestPairScan()
    {
        var used = new bool[_values.Length];
        var sum = 0;

        for (var pair = 0; pair < _values.Length / PairSize; pair++)
        {
            sum += MarkSmallestUnusedPair(_values, used);
        }

        return sum;
    }

    private static int MarkSmallestUnusedPair(int[] values, bool[] used)
    {
        var (firstIndex, secondIndex) = FindTwoSmallestUnusedIndices(values, used);
        return MarkPairUsedAndSum(values, used, firstIndex, secondIndex);
    }

    private static (int FirstIndex, int SecondIndex) FindTwoSmallestUnusedIndices(int[] values, bool[] used)
    {
        var firstIndex = -1;
        var secondIndex = -1;

        for (var i = 0; i < values.Length; i++)
        {
            if (used[i])
            {
                continue;
            }

            if (firstIndex < 0 || values[i] < values[firstIndex])
            {
                secondIndex = firstIndex;
                firstIndex = i;
            }
            else if (secondIndex < 0 || values[i] < values[secondIndex])
            {
                secondIndex = i;
            }
        }

        return (firstIndex, secondIndex);
    }

    private static int MarkPairUsedAndSum(int[] values, bool[] used, int firstIndex, int secondIndex)
    {
        used[firstIndex] = true;
        used[secondIndex] = true;
        return values[firstIndex];
    }

    [Benchmark]
    public int MergeSortPairSum()
    {
        var sorted = _values.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var sum = 0;
        for (var i = 0; i < sorted.Length; i += PairSize)
        {
            sum += sorted[i];
        }

        return sum;
    }
}
