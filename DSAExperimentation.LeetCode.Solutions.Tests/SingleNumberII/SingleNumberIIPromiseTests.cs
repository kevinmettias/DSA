using DSAExperimentation.LeetCode.Solutions.SingleNumberII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SingleNumberII;

public sealed partial class SingleNumberIIPromiseTests
{
    public static TheoryData<int[], bool> Inputs =>
        new()
        {
            { [2, 2, 3, 2], true },
            { [0, 1, 0, 1, 0, 1, 99], true },
            { [1, 1, 1, 1, 2], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenOneNumberIsSingle(int[] nums, bool expected)
        => Assert.Equal(expected, SingleNumberIIPromise.IsKeptBy(nums));
}
