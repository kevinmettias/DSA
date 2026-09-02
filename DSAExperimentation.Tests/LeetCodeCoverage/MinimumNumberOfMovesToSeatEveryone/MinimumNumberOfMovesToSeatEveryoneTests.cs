using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNumberOfMovesToSeatEveryone;

// LeetCode 2037. Minimum Number of Moves to Seat Everyone: sort both seats and
// students with this repo's MergeSort over ArrayIndexedSequence (the same
// composition ArrayPartitionTests already uses), then sum the absolute difference
// at each matched index - sorting both arrays and pairing by rank always minimizes
// the total number of moves for this 1-D matching problem.
public sealed partial class MinimumNumberOfMovesToSeatEveryoneTests
{
    [Fact]
    public void MinMovesToSeat_ClassicExample_ReturnsMinimumMoves()
    {
        int[] seats = [3, 1, 5];
        int[] students = [2, 7, 4];

        var actual = MinMovesToSeat(seats, students);
        Assert.Equal(4, actual);
    }

    [Fact]
    public void MinMovesToSeat_AlreadyMatchingMultiset_ReturnsZero()
    {
        int[] seats = [4, 1, 5, 9];
        int[] students = [1, 4, 9, 5];

        var actual = MinMovesToSeat(seats, students);
        Assert.Equal(0, actual);
    }

    private static int MinMovesToSeat(int[] seats, int[] students)
    {
        var sortedSeats = seats.ToArray();
        var sortedStudents = students.ToArray();
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
