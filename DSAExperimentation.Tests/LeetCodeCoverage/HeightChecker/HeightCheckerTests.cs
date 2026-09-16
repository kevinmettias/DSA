using DSAExperimentation.LeetCode.HeightChecker;

namespace DSAExperimentation.Tests.LeetCodeCoverage.HeightChecker;

// Harness only: both strategies live in HeightCheckerSolution, so the insertion-sort
// baseline the benchmark used to hide is asserted against the same examples as the
// MergeSort composition.
public sealed partial class HeightCheckerTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [1, 1, 4, 2, 1, 3], 3 },
            { [5, 1, 2, 3, 4], 5 },
            { [1, 2, 3, 4, 5], 0 },
            { [1], 0 },
            { [2, 1], 2 },
            { [3, 3, 3], 0 },
            { [5, 4, 3, 2, 1], 4 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountMismatchesByInsertionSort_LeetCodeExamples_ReturnsMismatchCount(int[] heights, int expected) =>
        Assert.Equal(expected, HeightCheckerSolution.CountMismatchesByInsertionSort(heights));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountMismatchesByMergeSort_LeetCodeExamples_ReturnsMismatchCount(int[] heights, int expected) =>
        Assert.Equal(expected, HeightCheckerSolution.CountMismatchesByMergeSort(heights));
}
