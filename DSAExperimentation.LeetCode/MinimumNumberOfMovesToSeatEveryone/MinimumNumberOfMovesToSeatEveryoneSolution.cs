using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.MinimumNumberOfMovesToSeatEveryone;

// LeetCode 2037. Minimum Number of Moves to Seat Everyone: each move shifts one
// student by one position, so seating everyone costs the total distance of
// whatever seat-to-student matching is chosen. Sorting both arrays and pairing by
// rank always minimizes that total for this 1-D matching - any crossing pair can
// be uncrossed without increasing the sum.
//
// Both strategies therefore agree on the pairing and differ only in how they sort:
//
// - MinMovesToSeatBySelectionSort repeatedly extracts the minimum remaining
//   element, an O(n^2) sort written in plain BCL. It is the "what you would write
//   without this repo" arm, previously the benchmark's unasserted baseline.
// - MinMovesToSeatByMergeSort uses this repo's own MergeSort over an
//   ArrayIndexedSequence, the same composition ArrayPartition uses for LC 561.
internal static class MinimumNumberOfMovesToSeatEveryoneSolution
{
    public static int MinMovesToSeatBySelectionSort(int[] seats, int[] students)
    {
        var sortedSeats = SelectionSorted(seats);
        var sortedStudents = SelectionSorted(students);

        return RankPairedDistance(sortedSeats, sortedStudents);
    }

    // A copy of the input ordered by repeatedly swapping the smallest remaining
    // element into place - no sorting algorithm beyond the two nested scans.
    private static int[] SelectionSorted(int[] source)
    {
        var values = source.ToArray();

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

        return values;
    }

    public static int MinMovesToSeatByMergeSort(int[] seats, int[] students)
    {
        var sortedSeats = MergeSorted(seats);
        var sortedStudents = MergeSorted(students);

        return RankPairedDistance(sortedSeats, sortedStudents);
    }

    // A copy of the input ordered by this repo's MergeSort, which sorts an
    // ArrayIndexedSequence view of the array in place.
    private static int[] MergeSorted(int[] source)
    {
        var values = source.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(values));

        return values;
    }

    // The k-th nearest seat takes the k-th nearest student; the moves are the sum
    // of those index-wise distances.
    private static int RankPairedDistance(int[] sortedSeats, int[] sortedStudents)
    {
        var moves = 0;

        for (var i = 0; i < sortedSeats.Length; i++)
        {
            moves += Math.Abs(sortedSeats[i] - sortedStudents[i]);
        }

        return moves;
    }
}
