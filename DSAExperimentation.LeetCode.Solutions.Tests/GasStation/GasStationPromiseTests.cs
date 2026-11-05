using DSAExperimentation.LeetCode.Solutions.GasStation;

namespace DSAExperimentation.LeetCode.Solutions.Tests.GasStation;

public sealed partial class GasStationPromiseTests
{
    public static TheoryData<int[], int[], bool> Inputs =>
        new()
        {
            { [1, 2, 3, 4, 5], [3, 4, 5, 1, 2], true },
            { [2, 3, 4], [3, 4, 3], true },
            { [1, 1, 1, 1], [1, 1, 1, 1], false },
            { [1, 0], [0, 0], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenAtMostOneStationCanStartALap(int[] gas, int[] cost, bool expected)
        => Assert.Equal(expected, GasStationPromise.IsKeptBy(gas, cost));
}
