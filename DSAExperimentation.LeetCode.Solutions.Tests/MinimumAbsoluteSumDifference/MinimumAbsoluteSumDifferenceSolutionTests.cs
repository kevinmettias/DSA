using DSAExperimentation.LeetCode.Solutions.MinimumAbsoluteSumDifference;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumAbsoluteSumDifference;

// Harness only. Both strategies are MinimumAbsoluteSumDifferenceSolution's - this
// file pins them to two cases the original
// coverage left untested: a single-element input (there is nothing else in nums1 to
// swap in, so the one term stands) and an input where the best replacement sits at
// the very end of the sorted copy, which is the boundary the lower-bound probe's
// insertion == Length branch exists for.
public sealed partial class MinimumAbsoluteSumDifferenceSolutionTests
{
    public static TheoryData<int[], int[], int> Cases =>
        new()
        {
            // One element: the only replacement available is the value itself.
            { [3], [9], 6 },
            // The closest replacement for 100 is the largest value in nums1, which
            // sits past every entry the lower bound can land on.
            { [1, 2, 90], [100, 2, 90], 10 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinAbsoluteSumDiffByFullRescan_Cases_ReturnsMinimumSum(
        int[] nums1, int[] nums2, int expected)
    {
        var actual = MinimumAbsoluteSumDifferenceSolution.MinAbsoluteSumDiffByFullRescan(nums1, nums2);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinAbsoluteSumDiffBySortedBinarySearch_Cases_ReturnsMinimumSum(
        int[] nums1, int[] nums2, int expected)
    {
        var actual = MinimumAbsoluteSumDifferenceSolution.MinAbsoluteSumDiffBySortedBinarySearch(nums1, nums2);

        Assert.Equal(expected, actual);
    }
}
