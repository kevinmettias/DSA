using DSAExperimentation.LeetCode.Solutions.NextGreaterElementI;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NextGreaterElementI;

// Harness only. Both strategies are NextGreaterElementISolution's - this file pins
// them to a strictly decreasing nums2, where no queried element has a greater one to
// its right. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class NextGreaterElementISolutionTests
{
    public static TheoryData<int[], int[], int[]> Cases =>
        new()
        {
            { [4, 3], [4, 3, 2, 1], [-1, -1] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void NextGreaterElementByPerQueryRescan_Cases_ReturnsPerElementAnswers(
        int[] nums1, int[] nums2, int[] expected)
    {
        var actual = NextGreaterElementISolution.NextGreaterElementByPerQueryRescan(nums1, nums2);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void NextGreaterElementByMonotonicStackSweep_Cases_ReturnsPerElementAnswers(
        int[] nums1, int[] nums2, int[] expected)
    {
        var actual = NextGreaterElementISolution.NextGreaterElementByMonotonicStackSweep(nums1, nums2);

        Assert.Equal(expected, actual);
    }
}
