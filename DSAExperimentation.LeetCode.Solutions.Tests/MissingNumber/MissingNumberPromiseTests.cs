using DSAExperimentation.LeetCode.Solutions.MissingNumber;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MissingNumber;

public sealed partial class MissingNumberPromiseTests
{
    public static TheoryData<int[], bool> Inputs =>
        new()
        {
            { [3, 0, 1], true },
            { [0, 1], true },
            { [10000, 9999], false },
            { [0, 0], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForDistinctNumbersFromZeroToN(int[] nums, bool expected)
        => Assert.Equal(expected, MissingNumberPromise.IsKeptBy(nums));
}
