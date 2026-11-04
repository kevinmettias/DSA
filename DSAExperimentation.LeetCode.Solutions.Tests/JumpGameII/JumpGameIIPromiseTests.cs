using DSAExperimentation.LeetCode.Solutions.JumpGameII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.JumpGameII;

public sealed partial class JumpGameIIPromiseTests
{
    public static TheoryData<int[], bool> Inputs =>
        new()
        {
            { [2, 3, 1, 1, 4], true },
            { [2, 3, 0, 1, 4], true },
            { [0], true },
            { [1, 0, 1], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenTheLastIndexIsReachable(int[] nums, bool expected)
        => Assert.Equal(expected, JumpGameIIPromise.IsKeptBy(nums));
}
