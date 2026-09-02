using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Number of Groups With Increasing Length (LC 2790): the whole algorithm is
// "sort usageLimits ascending, then one linear greedy sweep" (MaximumNumberOfGroupsWith
// IncreasingLengthTests' approach), so the only thing worth contrasting is the sort
// itself - the textbook O(n^2) insertion sort of a copy (HeightCheckerBenchmarks'
// precedent) vs. this repo's own MergeSort over ArrayIndexedSequence (O(n log n)).
// Both arms run the identical greedy sweep afterward.
[MemoryDiagnoser]
public class MaximumNumberOfGroupsWithIncreasingLengthBenchmarks
{
    private const int RandomSeed = 2790; // LC problem number
    private const int UsageLimitUpperBoundExclusive = 1_000;

    [Params(200, 5_000)]
    public int Length;

    private int[] _usageLimits = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _usageLimits = Enumerable.Range(0, Length).Select(_ => random.Next(1, UsageLimitUpperBoundExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int InsertionSortThenGreedySweep()
    {
        var sorted = _usageLimits.ToArray();

        for (var i = 1; i < sorted.Length; i++)
        {
            InsertOne(sorted, i);
        }

        return GreedySweep(sorted);
    }

    private static void InsertOne(int[] values, int index)
    {
        var value = values[index];
        var j = index - 1;

        while (j >= 0 && values[j] > value)
        {
            values[j + 1] = values[j];
            j--;
        }

        values[j + 1] = value;
    }

    [Benchmark]
    public int MergeSortThenGreedySweep()
    {
        var sorted = _usageLimits.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        return GreedySweep(sorted);
    }

    private static int GreedySweep(int[] sorted)
    {
        long available = 0;
        var groups = 0;
        var neededSize = 1;

        foreach (var limit in sorted)
        {
            available += limit;

            if (available >= neededSize)
            {
                groups++;
                available -= neededSize;
                neededSize++;
            }
        }

        return groups;
    }
}
