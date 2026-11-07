using DSAExperimentation.LeetCode.Solutions.CouplesHoldingHands;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CouplesHoldingHands;

public sealed partial class CouplesHoldingHandsPromiseTests
{
    public static TheoryData<int[], bool> Inputs =>
        new()
        {
            { [0, 2, 1, 3], true },
            { [3, 2, 0, 1], true },
            { [0, 1, 2, 2], false },
            { [0, 1, 2, 4], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenTheRowSeatsEachPersonOnce(int[] row, bool expected)
        => Assert.Equal(expected, CouplesHoldingHandsPromise.IsKeptBy(row));
}
