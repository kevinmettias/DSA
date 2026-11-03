using DSAExperimentation.LeetCode.TwoSum;

namespace DSAExperimentation.LeetCode.Tests.TwoSum;

public sealed partial class TwoSumPromiseTests
{
    public static TheoryData<int[], int, bool> Inputs =>
        new()
        {
            { [2, 7, 11, 15], 9, true },
            { [3, 3], 6, true },
            { [1, 2, 3], 100, false },
            { [1, 2, 3, 4], 5, false },
            { [3], 6, false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenExactlyOnePairReachesTheTarget(int[] nums, int target, bool expected)
        => Assert.Equal(expected, TwoSumPromise.IsKeptBy(nums, target));
}
