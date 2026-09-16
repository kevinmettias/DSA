using DSAExperimentation.LeetCode.HouseRobberIV;

namespace DSAExperimentation.Tests.LeetCodeCoverage.HouseRobberIV;

// Harness only. Both strategies are HouseRobberIVSolution's - this file just pins
// them to LeetCode's published examples, plus the "requiredHouseCount equals the
// maximum number of non-adjacent houses" boundary the original test carried and
// three further cases the original arms never reached: a single house,
// requiredHouseCount = 1, and a run where the only requiredHouseCount non-adjacent
// choices all sit at the maximum.
public sealed class HouseRobberIVTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [2, 3, 5, 9], 2, 5 },
            { [2, 7, 9, 3, 1], 2, 2 },
            { [6, 1, 8], 2, 8 },
            { [4], 1, 4 },
            { [5, 3, 4, 7], 1, 3 },
            { [1, 2, 3, 4, 5, 6], 3, 5 },
            { [9, 1, 9, 1, 9], 3, 9 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCapabilityByLinearScan_LeetCodeExamples_ReturnsSmallestFeasibleCapability(
        int[] nums, int requiredHouseCount, int expected)
    {
        var actual = HouseRobberIVSolution.MinCapabilityByLinearScan(nums, requiredHouseCount);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCapabilityBySequenceLowerBound_LeetCodeExamples_ReturnsSmallestFeasibleCapability(
        int[] nums, int requiredHouseCount, int expected)
    {
        var actual = HouseRobberIVSolution.MinCapabilityBySequenceLowerBound(nums, requiredHouseCount);

        Assert.Equal(expected, actual);
    }
}
