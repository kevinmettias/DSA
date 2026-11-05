using DSAExperimentation.LeetCode.Solutions.HeightChecker;

namespace DSAExperimentation.LeetCode.Solutions.Tests.HeightChecker;

// Harness only: both strategies live in HeightCheckerSolution, so the insertion-sort
// baseline the benchmark used to hide is asserted against the same cases as the
// MergeSort composition.
public sealed partial class HeightCheckerSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [2, 1], 2 },
            { [3, 3, 3], 0 },
            { [5, 4, 3, 2, 1], 4 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountMismatchesByInsertionSort_Cases_ReturnsMismatchCount(int[] heights, int expected) =>
        Assert.Equal(expected, HeightCheckerSolution.CountMismatchesByInsertionSort(heights));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountMismatchesByMergeSort_Cases_ReturnsMismatchCount(int[] heights, int expected) =>
        Assert.Equal(expected, HeightCheckerSolution.CountMismatchesByMergeSort(heights));
}
