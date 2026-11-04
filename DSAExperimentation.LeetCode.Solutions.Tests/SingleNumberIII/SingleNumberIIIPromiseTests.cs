using DSAExperimentation.LeetCode.Solutions.SingleNumberIII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SingleNumberIII;

public sealed partial class SingleNumberIIIPromiseTests
{
    public static TheoryData<int[], bool> Inputs =>
        new()
        {
            { [1, 2, 1, 3, 2, 5], true },
            { [0, 1], true },
            { [1, 2, 3], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenTwoNumbersAreSingle(int[] nums, bool expected)
        => Assert.Equal(expected, SingleNumberIIIPromise.IsKeptBy(nums));
}
