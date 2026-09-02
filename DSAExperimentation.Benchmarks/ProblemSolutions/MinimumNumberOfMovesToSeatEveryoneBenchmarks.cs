using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Number of Moves to Seat Everyone (LC 2037): an O(n^2) selection sort of
// both arrays (repeatedly extracting the minimum remaining element, no real
// sorting algorithm - the same "no sort" brute-force shape ArrayPartitionBenchmarks
// uses for LC 561) vs this repo's own O(n log n) MergeSort over
// ArrayIndexedSequence, both followed by summing the index-wise absolute
// difference between the two sorted arrays.
[MemoryDiagnoser]
public class MinimumNumberOfMovesToSeatEveryoneBenchmarks
{
    private const int RandomSeed = 2037;
    private const int MaxSeatPosition = 1_000_000;

    [Params(200, 5_000)]
    public int Length;

    private int[] _seats = null!;
    private int[] _students = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _seats = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxSeatPosition)).ToArray();
        _students = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxSeatPosition)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int SelectionSortPairSum()
    {
        var sortedSeats = _seats.ToArray();
        var sortedStudents = _students.ToArray();
        SelectionSort(sortedSeats);
        SelectionSort(sortedStudents);

        var moves = 0;
        for (var i = 0; i < sortedSeats.Length; i++)
        {
            moves += Math.Abs(sortedSeats[i] - sortedStudents[i]);
        }

        return moves;
    }

    private static void SelectionSort(int[] values)
    {
        for (var i = 0; i < values.Length; i++)
        {
            var smallest = i;

            for (var j = i + 1; j < values.Length; j++)
            {
                if (values[j] < values[smallest])
                {
                    smallest = j;
                }
            }

            (values[i], values[smallest]) = (values[smallest], values[i]);
        }
    }

    [Benchmark]
    public int MergeSortPairSum()
    {
        var sortedSeats = _seats.ToArray();
        var sortedStudents = _students.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sortedSeats));
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sortedStudents));

        var moves = 0;
        for (var i = 0; i < sortedSeats.Length; i++)
        {
            moves += Math.Abs(sortedSeats[i] - sortedStudents[i]);
        }

        return moves;
    }
}
