using DSAExperimentation.LeetCode.Solutions.MajorityElement;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MajorityElement;

public sealed partial class MajorityElementPromiseTests
{
    public static TheoryData<int[], bool> Inputs =>
        new()
        {
            { [3, 2, 3], true },
            { [2, 2, 1, 1, 1, 2, 2], true },
            { [1, 1, 1, 1, 2, 2, 3, 3, 3, 3, 3], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenAnElementIsInTheMajority(int[] nums, bool expected)
        => Assert.Equal(expected, MajorityElementPromise.IsKeptBy(nums));
}
