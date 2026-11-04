using DSAExperimentation.LeetCode.Solutions.MaximumSegmentSumAfterRemovals;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumSegmentSumAfterRemovals;

public sealed partial class MaximumSegmentSumAfterRemovalsPromiseTests
{
    public static TheoryData<int[], int[], bool> Inputs =>
        new()
        {
            { [1, 2, 5, 6, 1], [0, 3, 2, 4, 1], true },
            { [1, 2], [0, 0], false },
            { [1, 2], [0, 1, 2], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenEveryIndexIsRemovedOnce(int[] nums, int[] removeQueries, bool expected)
        => Assert.Equal(expected, MaximumSegmentSumAfterRemovalsPromise.IsKeptBy(nums, removeQueries));
}
