using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfFlowersInFullBloom;

// LeetCode 2251. Number of Flowers in Full Bloom: a flower is in bloom at time t
// exactly when start <= t <= end, so the count in bloom at t is
// (# starts <= t) - (# ends < t). Sorting the start/end arrays once with this
// repo's own MergeSort and then answering each query with BinarySearch's
// UpperBound (count of starts <= t)/LowerBound (count of ends < t) turns the
// textbook O(n*m) per-person scan into O((n+m) log n) - the same
// sort-then-bound composition HowManyNumbersAreSmallerThanTheCurrentNumberTests
// already proves out, applied to two arrays instead of one.
public sealed partial class NumberOfFlowersInFullBloomTests
{
    [Fact]
    public void FullBloomFlowers_ClassicExampleOne_ReturnsCountPerPerson()
    {
        int[][] flowers = [[1, 6], [3, 7], [9, 12], [4, 13]];
        int[] persons = [2, 3, 7, 11];

        var counts = FullBloomFlowers(flowers, persons);

        Assert.Equal([1, 2, 2, 2], counts);
    }

    [Fact]
    public void FullBloomFlowers_ClassicExampleTwo_ReturnsCountPerPerson()
    {
        int[][] flowers = [[1, 10], [3, 3]];
        int[] persons = [3, 3, 2];

        var counts = FullBloomFlowers(flowers, persons);

        Assert.Equal([2, 2, 1], counts);
    }

    [Fact]
    public void FullBloomFlowers_PersonBeforeEveryFlowerStarts_ReturnsZero()
    {
        int[][] flowers = [[5, 10]];
        int[] persons = [1];

        var counts = FullBloomFlowers(flowers, persons);

        Assert.Equal([0], counts);
    }

    private static int[] FullBloomFlowers(int[][] flowers, int[] persons)
    {
        var (starts, ends) = ExtractStartsAndEnds(flowers);

        SortTimes(starts);
        SortTimes(ends);

        var startSequence = new ArraySequence<int>(starts);
        var endSequence = new ArraySequence<int>(ends);

        return CountBloomsPerPerson(persons, startSequence, endSequence);
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

    private static int[] CountBloomsPerPerson(int[] persons, ArraySequence<int> startSequence, ArraySequence<int> endSequence)
    {
        var counts = new int[persons.Length];

        for (var i = 0; i < persons.Length; i++)
        {
            counts[i] = CountBloomingAt(startSequence, endSequence, persons[i]);
        }

        return counts;
    }

    private static int CountBloomingAt(ArraySequence<int> startSequence, ArraySequence<int> endSequence, int time)
    {
        var bloomedByNow = BinarySearch.UpperBound<int, ArraySequence<int>>(startSequence, time);
        var wiltedByNow = BinarySearch.LowerBound<int, ArraySequence<int>>(endSequence, time);
        return bloomedByNow - wiltedByNow;
    }
}
