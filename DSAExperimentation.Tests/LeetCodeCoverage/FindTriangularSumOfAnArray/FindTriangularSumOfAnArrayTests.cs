using DSAExperimentation.LeetCode.FindTriangularSumOfAnArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTriangularSumOfAnArray;

// Harness only. Both strategies are FindTriangularSumOfAnArraySolution's - this file
// just pins them to LeetCode's published examples plus the cases that exercise the
// mod-10 wrap and the single-element short circuit where no reduction runs at all.
public sealed class FindTriangularSumOfAnArrayTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [1, 2, 3, 4, 5], 8 },
            { [5], 5 },
            { [7, 8], 5 },
            { [9, 9], 8 },
            { [0], 0 },
            { [1, 1, 1, 1], 8 },
            { [3, 4, 5, 7], 7 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void TriangularSumByInPlaceArray_LeetCodeExamples_ReturnsFinalDigit(int[] nums, int expected) =>
        Assert.Equal(expected, FindTriangularSumOfAnArraySolution.TriangularSumByInPlaceArray(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void TriangularSumByDynamicArray_LeetCodeExamples_ReturnsFinalDigit(int[] nums, int expected) =>
        Assert.Equal(expected, FindTriangularSumOfAnArraySolution.TriangularSumByDynamicArray(nums));
}
