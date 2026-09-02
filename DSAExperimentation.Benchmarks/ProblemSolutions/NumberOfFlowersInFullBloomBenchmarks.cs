using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Number of Flowers in Full Bloom (LC 2251): the textbook O(n*m) baseline scans
// every flower for every person - vs. sorting the start/end arrays once with this
// repo's own MergeSort and answering each person with two BinarySearch bounds
// (UpperBound on starts, LowerBound on ends), O((n+m) log n) overall - the same
// sort-then-bound shape HowManyNumbersAreSmallerThanTheCurrentNumberBenchmarks
// already proves out.
[MemoryDiagnoser]
public class NumberOfFlowersInFullBloomBenchmarks
{
    private const int RandomSeed = 2251; // LC problem number
    private const int MaxTimeExclusive = 1_000_000;
    private const int MaxFlowerLengthExclusive = 1_000;

    [Params(200, 2_000)]
    public int Count;

    private int[][] _flowers = null!;
    private int[] _persons = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        _flowers = new int[Count][];
        for (var i = 0; i < Count; i++)
        {
            var start = random.Next(0, MaxTimeExclusive);
            var length = random.Next(1, MaxFlowerLengthExclusive);
            _flowers[i] = [start, start + length];
        }

        _persons = Enumerable.Range(0, Count).Select(_ => random.Next(0, MaxTimeExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        var total = 0;

        foreach (var time in _persons)
        {
            foreach (var flower in _flowers)
            {
                if (flower[0] <= time && time <= flower[1])
                {
                    total++;
                }
            }
        }

        return total;
    }

    [Benchmark]
    public int SortThenBinarySearch()
    {
        var (starts, ends) = ExtractStartsAndEnds(_flowers);

        SortTimes(starts);
        SortTimes(ends);

        var startSequence = new ArraySequence<int>(starts);
        var endSequence = new ArraySequence<int>(ends);

        return SumBloomsAcrossPersons(_persons, startSequence, endSequence);
    }

    private static (int[] Starts, int[] Ends) ExtractStartsAndEnds(int[][] flowers)
    {
        var starts = new int[flowers.Length];
        var ends = new int[flowers.Length];

        for (var i = 0; i < flowers.Length; i++)
        {
            starts[i] = flowers[i][0];
            ends[i] = flowers[i][1];
        }

        return (starts, ends);
    }

    private static void SortTimes(int[] times)
    {
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(times));
    }

    private static int SumBloomsAcrossPersons(int[] persons, ArraySequence<int> startSequence, ArraySequence<int> endSequence)
    {
        var total = 0;

        foreach (var time in persons)
        {
            total += CountBloomingAt(startSequence, endSequence, time);
        }

        return total;
    }

    private static int CountBloomingAt(ArraySequence<int> startSequence, ArraySequence<int> endSequence, int time)
    {
        var bloomedByNow = BinarySearch.UpperBound<int, ArraySequence<int>>(startSequence, time);
        var wiltedByNow = BinarySearch.LowerBound<int, ArraySequence<int>>(endSequence, time);
        return bloomedByNow - wiltedByNow;
    }
}
