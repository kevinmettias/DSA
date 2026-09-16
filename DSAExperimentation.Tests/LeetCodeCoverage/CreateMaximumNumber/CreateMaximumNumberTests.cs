using DSAExperimentation.LeetCode.CreateMaximumNumber;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CreateMaximumNumber;

// Harness only: both strategies live in CreateMaximumNumberSolution and are
// asserted against the same examples.
public sealed class CreateMaximumNumberTests
{
    public static TheoryData<int[], int[], int, int[]> Examples =>
        new()
        {
            { [3, 4, 6, 5], [9, 1, 2, 5, 8, 3], 5, [9, 8, 6, 5, 3] },
            { [6, 7], [6, 0, 4], 5, [6, 7, 6, 0, 4] },
            { [3, 9], [8, 9], 3, [9, 8, 9] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxNumberByNaiveScan_LeetCodeExamples_ReturnsLargestMergedDigits(
        int[] nums1, int[] nums2, int digitCount, int[] expected)
    {
        var actual = CreateMaximumNumberSolution.MaxNumberByNaiveScan(nums1, nums2, digitCount);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxNumberByMonotonicStack_LeetCodeExamples_ReturnsLargestMergedDigits(
        int[] nums1, int[] nums2, int digitCount, int[] expected)
    {
        var actual = CreateMaximumNumberSolution.MaxNumberByMonotonicStack(nums1, nums2, digitCount);
        Assert.Equal(expected, actual);
    }
}
