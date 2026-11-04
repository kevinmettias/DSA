using DSAExperimentation.LeetCode.Solutions.TwoSumIIInputArrayIsSorted;

namespace DSAExperimentation.LeetCode.Solutions.Tests.TwoSumIIInputArrayIsSorted;

public sealed partial class TwoSumIIInputArrayIsSortedPromiseTests
{
    public static TheoryData<int[], int, bool> Inputs =>
        new()
        {
            { [2, 7, 11, 15], 9, true },
            { [-1, 0], -1, true },
            { [0, 0, 0, 0], 0, false },
            { [1, 2, 3, 4], 5, false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenExactlyOnePairReachesTheTarget(int[] nums, int target, bool expected)
        => Assert.Equal(expected, TwoSumIIInputArrayIsSortedPromise.IsKeptBy(nums, target));
}
