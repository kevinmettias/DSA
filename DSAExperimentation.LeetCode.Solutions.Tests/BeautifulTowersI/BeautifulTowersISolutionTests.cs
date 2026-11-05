using DSAExperimentation.LeetCode.Solutions.BeautifulTowersI;

namespace DSAExperimentation.LeetCode.Solutions.Tests.BeautifulTowersI;

// Harness only. Both strategies live in BeautifulTowersISolution - this file pins
// them to a single tower (nothing to
// clamp on either side of the peak), a strictly increasing array (the peak is the
// last index and the whole array is its own left run) and a flat array (every
// tower keeps its cap).
public sealed partial class BeautifulTowersISolutionTests
{
    public static TheoryData<int[], long> Cases =>
        new()
        {
            { [7], 7 },
            { [4, 4, 4, 4], 16 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaximumSumOfHeightsByBruteForce_Cases_ReturnsMaxSum(
        int[] maxHeights, long expected) =>
        Assert.Equal(expected, BeautifulTowersISolution.MaximumSumOfHeightsByBruteForce(maxHeights));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaximumSumOfHeightsByMonotonicStack_Cases_ReturnsMaxSum(
        int[] maxHeights, long expected) =>
        Assert.Equal(expected, BeautifulTowersISolution.MaximumSumOfHeightsByMonotonicStack(maxHeights));
}
