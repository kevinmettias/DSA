using DSAExperimentation.LeetCode.Solutions.HouseRobberIV;

namespace DSAExperimentation.LeetCode.Solutions.Tests.HouseRobberIV;

public sealed partial class HouseRobberIVPromiseTests
{
    public static TheoryData<int[], int, bool> Inputs =>
        new()
        {
            { [2, 3, 5, 9], 2, true },
            { [2, 7, 9, 3, 1], 2, true },
            { [1, 1, 1], 3, false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenKNonAdjacentHousesExist(int[] nums, int requiredHouseCount, bool expected)
        => Assert.Equal(expected, HouseRobberIVPromise.IsKeptBy(nums, requiredHouseCount));
}
