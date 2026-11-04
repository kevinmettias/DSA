using DSAExperimentation.LeetCode.Solutions.MakeArrayEmpty;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MakeArrayEmpty;

public sealed partial class MakeArrayEmptyPromiseTests
{
    public static TheoryData<int[], bool> Inputs =>
        new()
        {
            { [3, 4, -1], true },
            { [1, 2, 4, 3], true },
            { [10, 5, 5], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenTheValuesAreDistinct(int[] nums, bool expected)
        => Assert.Equal(expected, MakeArrayEmptyPromise.IsKeptBy(nums));
}
