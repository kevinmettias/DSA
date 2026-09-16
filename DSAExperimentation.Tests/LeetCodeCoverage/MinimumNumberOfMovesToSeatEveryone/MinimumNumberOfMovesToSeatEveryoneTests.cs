using DSAExperimentation.LeetCode.MinimumNumberOfMovesToSeatEveryone;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNumberOfMovesToSeatEveryone;

// Harness only. LeetCode 2037's three published examples plus the
// already-a-permutation case the original test carried, which is the one that
// catches a pairing that is not by rank. Both strategies live in
// MinimumNumberOfMovesToSeatEveryoneSolution and are asserted separately so a
// failure names the arm that broke.
public sealed partial class MinimumNumberOfMovesToSeatEveryoneTests
{
    public static TheoryData<int[], int[], int> Examples => new()
    {
        { [3, 1, 5], [2, 7, 4], 4 },
        { [4, 1, 5, 9], [1, 3, 2, 6], 7 },
        { [2, 2, 6, 6], [1, 3, 2, 6], 4 },
        { [4, 1, 5, 9], [1, 4, 9, 5], 0 },
    };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinMovesToSeatBySelectionSort_LeetCodeExamples_ReturnsMinimumMoves(
        int[] seats, int[] students, int expected)
    {
        var actual = MinimumNumberOfMovesToSeatEveryoneSolution.MinMovesToSeatBySelectionSort(seats, students);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinMovesToSeatByMergeSort_LeetCodeExamples_ReturnsMinimumMoves(
        int[] seats, int[] students, int expected)
    {
        var actual = MinimumNumberOfMovesToSeatEveryoneSolution.MinMovesToSeatByMergeSort(seats, students);

        Assert.Equal(expected, actual);
    }
}
