using DSAExperimentation.LeetCode.MaximumBalancedShipments;

namespace DSAExperimentation.LeetCode.Tests.MaximumBalancedShipments;

// Harness only. Both strategies are MaximumBalancedShipmentsSolution's - this file
// just pins them to LeetCode's published examples.
public sealed partial class MaximumBalancedShipmentsSolutionTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [2, 5, 1, 4, 3], 2 },
            { [4, 4], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxBalancedShipmentsByBruteForce_LeetCodeExamples_ReturnsMaxNonOverlappingBalancedCount(
        int[] weight, int expected) =>
        Assert.Equal(expected, MaximumBalancedShipmentsSolution.MaxBalancedShipmentsByBruteForce(weight));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxBalancedShipmentsByPreviousGreaterStack_LeetCodeExamples_ReturnsMaxNonOverlappingBalancedCount(
        int[] weight, int expected) =>
        Assert.Equal(expected, MaximumBalancedShipmentsSolution.MaxBalancedShipmentsByPreviousGreaterStack(weight));
}
