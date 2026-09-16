using DSAExperimentation.LeetCode.CountConnectedComponentsInLCMGraph;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountConnectedComponentsInLCMGraph;

// Harness only: both strategies are CountConnectedComponentsInLCMGraphSolution's.
// One test method per strategy over LeetCode's own examples, so a failure names
// the strategy that broke.
public sealed class CountConnectedComponentsInLCMGraphTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [2, 4, 8, 3, 9], 5, 4 },
            { [2, 4, 8, 3, 9, 12], 10, 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountComponentsByPairwiseLcmScan_LeetCodeExamples_ReturnsComponentCount(
        int[] nums, int threshold, int expected)
    {
        var actual = CountConnectedComponentsInLCMGraphSolution.CountComponentsByPairwiseLcmScan(nums, threshold);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountComponentsByMultipleUnion_LeetCodeExamples_ReturnsComponentCount(
        int[] nums, int threshold, int expected)
    {
        var actual = CountConnectedComponentsInLCMGraphSolution.CountComponentsByMultipleUnion(nums, threshold);
        Assert.Equal(expected, actual);
    }
}
