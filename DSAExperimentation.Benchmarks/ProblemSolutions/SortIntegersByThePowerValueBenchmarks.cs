using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Sort Integers by The Power Value (LC 1387): the textbook O(n^2) insertion sort
// of the (Power, Value) pairs (SortAnArrayBenchmarks/HeightCheckerBenchmarks
// precedent) vs. this repo's own MergeSort over ArrayIndexedSequence (O(n log n)),
// relying on ValueTuple<int,int>'s lexicographic IComparable to sort by Power
// first and Value second with no custom comparer. Both benchmarks recompute the
// same Collatz power for every candidate first, so the comparison isolates the
// sorting strategy rather than the power computation.
[MemoryDiagnoser]
public class SortIntegersByThePowerValueBenchmarks
{
    // Collatz step: even -> divide by CollatzDivisor; odd -> CollatzMultiplier * x + 1.
    private const int CollatzDivisor = 2;
    private const int CollatzMultiplier = 3;

    [Params(200, 5_000)]
    public int RangeLength;

    private int _lo;

    [GlobalSetup]
    public void Setup() => _lo = 1;

    [Benchmark(Baseline = true)]
    public int InsertionSort()
    {
        var pairs = BuildPairs();

        for (var i = 1; i < pairs.Length; i++)
        {
            var current = pairs[i];
            var j = i - 1;

            while (j >= 0 && Comparer<(int Power, int Value)>.Default.Compare(pairs[j], current) > 0)
            {
                pairs[j + 1] = pairs[j];
                j--;
            }

            pairs[j + 1] = current;
        }

        return pairs[^1].Value;
    }

    [Benchmark]
    public int MergeSortAscending()
    {
        var pairs = BuildPairs();

        MergeSort.Sort<(int Power, int Value), ArrayIndexedSequence<(int Power, int Value)>>(
            new ArrayIndexedSequence<(int Power, int Value)>(pairs));

        return pairs[^1].Value;
    }

    private (int Power, int Value)[] BuildPairs()
    {
        var pairs = new (int Power, int Value)[RangeLength];

        for (var i = 0; i < RangeLength; i++)
        {
            var value = _lo + i;
            pairs[i] = (PowerOf(value), value);
        }

        return pairs;
    }

    private static int PowerOf(int x)
    {
        var power = 0;

        while (x != 1)
        {
            x = x % CollatzDivisor == 0 ? x / CollatzDivisor : (CollatzMultiplier * x) + 1;
            power++;
        }

        return power;
    }
}
