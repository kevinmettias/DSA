using DSAExperimentation.LeetCode.Solutions.MinimumNumberOfMovesToSeatEveryone;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumNumberOfMovesToSeatEveryone;

// Harness only. The one case here is the already-a-permutation case the original
// test carried, which is the one that catches a pairing that is not by rank. Both
// strategies live in MinimumNumberOfMovesToSeatEveryoneSolution and are asserted
// separately so a failure names the arm that broke. LeetCode's published examples
// run in CatalogCaseTests.
public sealed partial class MinimumNumberOfMovesToSeatEveryoneSolutionTests
{
    public static TheoryData<int[], int[], int> Cases => new()
    {
        { [4, 1, 5, 9], [1, 4, 9, 5], 0 },
    };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinMovesToSeatBySelectionSort_Cases_ReturnsMinimumMoves(
        int[] seats, int[] students, int expected)
    {
        var actual = MinimumNumberOfMovesToSeatEveryoneSolution.MinMovesToSeatBySelectionSort(seats, students);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinMovesToSeatByMergeSort_Cases_ReturnsMinimumMoves(
        int[] seats, int[] students, int expected)
    {
        var actual = MinimumNumberOfMovesToSeatEveryoneSolution.MinMovesToSeatByMergeSort(seats, students);

        Assert.Equal(expected, actual);
    }
}
