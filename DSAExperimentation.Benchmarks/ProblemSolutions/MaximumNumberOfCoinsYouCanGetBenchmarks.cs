using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Number of Coins You Can Get (LC 1561): simulating the actual game round by
// round with a linear max/min scan per pick (baseline - O(n^2), the way most people
// solve this the first time) vs. this repo's own MergeSort over an
// ArrayIndexedSequence<int> (O(n log n)) followed by summing every second pile
// starting at index n - the same InsertionSort-vs-MergeSort contrast
// SortAnArrayBenchmarks already established, here with the picking arithmetic
// layered on top instead of returning the sorted array itself.
[MemoryDiagnoser]
public class MaximumNumberOfCoinsYouCanGetBenchmarks
{
    private const int RandomSeed = 1561; // LC problem number
    private const int MaxPileValueExclusive = 10_000;
    private const int GroupSize = 3;
    private const int PickStride = 2;

    [Params(300, 3_000)]
    public int PileCount;

    private int[] _piles = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _piles = Enumerable.Range(0, PileCount).Select(_ => random.Next(1, MaxPileValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int SimulateRoundsWithLinearScans()
    {
        var remaining = new List<int>(_piles);
        var total = 0;

        while (remaining.Count > 0)
        {
            RemoveMax(remaining);
            total += RemoveMax(remaining);
            RemoveMin(remaining);
        }

        return total;
    }

    private static int RemoveMax(List<int> values)
    {
        var maxIndex = 0;
        for (var i = 1; i < values.Count; i++)
        {
            if (values[i] > values[maxIndex])
            {
                maxIndex = i;
            }
        }

        var value = values[maxIndex];
        values.RemoveAt(maxIndex);
        return value;
    }

    private static void RemoveMin(List<int> values)
    {
        var minIndex = 0;
        for (var i = 1; i < values.Count; i++)
        {
            if (values[i] < values[minIndex])
            {
                minIndex = i;
            }
        }

        values.RemoveAt(minIndex);
    }

    [Benchmark]
    public int SortAscendingThenSumEveryOtherFromMiddle()
    {
        var piles = _piles.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(piles));

        var n = piles.Length / GroupSize;
        var total = 0;
        for (var i = 0; i < n; i++)
        {
            total += piles[n + (PickStride * i)];
        }

        return total;
    }
}
