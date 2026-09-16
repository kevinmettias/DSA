using DSAExperimentation.LeetCode.MinimumAbsoluteSumDifference;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumAbsoluteSumDifference;

// Harness only. Both strategies are MinimumAbsoluteSumDifferenceSolution's - this
// file pins them to LeetCode's three published examples plus two cases the original
// coverage left untested: a single-element input (there is nothing else in nums1 to
// swap in, so the one term stands) and an input where the best replacement sits at
// the very end of the sorted copy, which is the boundary the lower-bound probe's
// insertion == Length branch exists for.
public sealed class MinimumAbsoluteSumDifferenceTests
{
    public static TheoryData<int[], int[], int> Examples =>
        new()
        {
            // LeetCode example 1: replacing the 7 with the 5 drops |7-3| from 4 to 2.
            { [1, 7, 5], [2, 3, 5], 3 },
            // LeetCode example 2: already elementwise equal, so no swap can help.
            { [2, 4, 6, 8, 10], [2, 4, 6, 8, 10], 0 },
            // LeetCode example 3.
            { [1, 10, 4, 4, 2, 7], [9, 3, 5, 1, 7, 4], 20 },
            // One element: the only replacement available is the value itself.
            { [3], [9], 6 },
            // The closest replacement for 100 is the largest value in nums1, which
            // sits past every entry the lower bound can land on.
            { [1, 2, 90], [100, 2, 90], 10 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinAbsoluteSumDiffByFullRescan_LeetCodeExamples_ReturnsMinimumSum(
        int[] nums1, int[] nums2, int expected)
    {
        var actual = MinimumAbsoluteSumDifferenceSolution.MinAbsoluteSumDiffByFullRescan(nums1, nums2);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinAbsoluteSumDiffBySortedBinarySearch_LeetCodeExamples_ReturnsMinimumSum(
        int[] nums1, int[] nums2, int expected)
    {
        var actual = MinimumAbsoluteSumDifferenceSolution.MinAbsoluteSumDiffBySortedBinarySearch(nums1, nums2);

        Assert.Equal(expected, actual);
    }
}
