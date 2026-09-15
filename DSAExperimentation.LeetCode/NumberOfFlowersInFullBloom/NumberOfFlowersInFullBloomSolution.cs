using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.NumberOfFlowersInFullBloom;

// LeetCode 2251. Number of Flowers in Full Bloom: a flower [start, end] is in
// bloom at time t exactly when start <= t <= end, so the count in bloom at t is
// (# starts <= t) - (# ends < t). Each person arrives at one time and wants that
// count.
//
// Both strategies return the same int[] LeetCode asks for; they differ only in
// whether every flower is re-examined per person, or the two endpoint arrays are
// sorted once and each person answered by two bisections.
internal static class NumberOfFlowersInFullBloomSolution
{
    // The textbook O(n*m) answer: for every person, walk every flower and tally the
    // ones whose interval covers their arrival time. Deliberately plain BCL - it is
    // the arm the composed strategy below has to justify itself against.
    public static int[] FullBloomFlowersByPerPersonScan(int[][] flowers, int[] persons)
    {
        var counts = new int[persons.Length];

        for (var i = 0; i < persons.Length; i++)
        {
            counts[i] = CountCoveringFlowers(flowers, persons[i]);
        }

        return counts;
    }

    private static int CountCoveringFlowers(int[][] flowers, int time)
    {
        var count = 0;

        foreach (var flower in flowers)
        {
            if (flower[0] <= time && time <= flower[1])
            {
                count++;
            }
        }

        return count;
    }

    // Starts and ends are independent once the interval is split: sorting each with
    // this repo's own MergeSort and then asking BinarySearch for UpperBound on the
    // starts (count of starts <= t) and LowerBound on the ends (count of ends < t)
    // gives the difference directly, turning the per-person scan into O((n+m) log n)
    // - the same sort-then-bound composition
    // HowManyNumbersAreSmallerThanTheCurrentNumber proves out, applied to two arrays
    // instead of one.
    public static int[] FullBloomFlowersBySortedBounds(int[][] flowers, int[] persons)
    {
        var (starts, ends) = ExtractStartsAndEnds(flowers);
        SortEndpoints(starts, ends);

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

    // Both endpoint arrays are sorted once here, which is what lets each person be
    // answered by two bisections instead of a re-scan.
    private static void SortEndpoints(int[] starts, int[] ends)
    {
        SortTimes(starts);
        SortTimes(ends);
    }

    private static void SortTimes(int[] times) => MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(times));

    private static int[] CountBloomsPerPerson(
        int[] persons, ArraySequence<int> startSequence, ArraySequence<int> endSequence)
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
