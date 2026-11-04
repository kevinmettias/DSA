using DSAExperimentation.LeetCode.Solutions.NRepeatedElementInSize2NArray;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NRepeatedElementInSize2NArray;

public sealed partial class NRepeatedElementInSize2NArrayPromiseTests
{
    public static TheoryData<int[], bool> Inputs =>
        new()
        {
            { [1, 2, 3, 3], true },
            { [5, 1, 5, 2, 5, 3, 5, 4], true },
            { [9876, 5432, 1234, 8765, 4321, 1234, 9876, 5432, 8765, 4321, 9876, 1234], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenOneElementFillsHalf(int[] nums, bool expected)
        => Assert.Equal(expected, NRepeatedElementInSize2NArrayPromise.IsKeptBy(nums));
}
