using DSAExperimentation.LeetCode.NextGreaterElementI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NextGreaterElementI;

// Harness only. Both strategies are NextGreaterElementISolution's - this file just
// pins them to LeetCode's published examples.
public sealed partial class NextGreaterElementITests
{
    public static TheoryData<int[], int[], int[]> Examples =>
        new()
        {
            { [4, 1, 2], [1, 3, 4, 2], [-1, 3, -1] },
            { [2, 4], [1, 2, 3, 4], [3, -1] },
            { [4, 3], [4, 3, 2, 1], [-1, -1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void NextGreaterElementByPerQueryRescan_LeetCodeExamples_ReturnsPerElementAnswers(
        int[] nums1, int[] nums2, int[] expected)
    {
        var actual = NextGreaterElementISolution.NextGreaterElementByPerQueryRescan(nums1, nums2);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void NextGreaterElementByMonotonicStackSweep_LeetCodeExamples_ReturnsPerElementAnswers(
        int[] nums1, int[] nums2, int[] expected)
    {
        var actual = NextGreaterElementISolution.NextGreaterElementByMonotonicStackSweep(nums1, nums2);

        Assert.Equal(expected, actual);
    }
}
