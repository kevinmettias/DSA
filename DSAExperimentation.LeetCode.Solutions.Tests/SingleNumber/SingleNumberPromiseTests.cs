using DSAExperimentation.LeetCode.Solutions.SingleNumber;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SingleNumber;

public sealed partial class SingleNumberPromiseTests
{
    public static TheoryData<int[], bool> Inputs =>
        new()
        {
            { [2, 2, 1], true },
            { [4, 1, 2, 1, 2], true },
            { [1, 2, 3, 1], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenOneNumberIsSingle(int[] nums, bool expected)
        => Assert.Equal(expected, SingleNumberPromise.IsKeptBy(nums));
}
