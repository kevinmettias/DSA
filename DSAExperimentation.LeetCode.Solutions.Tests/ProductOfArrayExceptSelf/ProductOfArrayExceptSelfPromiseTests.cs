using DSAExperimentation.LeetCode.Solutions.ProductOfArrayExceptSelf;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ProductOfArrayExceptSelf;

public sealed partial class ProductOfArrayExceptSelfPromiseTests
{
    public static TheoryData<int[], bool> Inputs =>
        new()
        {
            { [1, 2, 3, 4], true },
            { [-1, 1, 0, -3, 3], true },
            { [-30, -29, -28, -27, -26, -25, -24, -23, -22, -21], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenEveryProductFitsAnInt(int[] nums, bool expected)
        => Assert.Equal(expected, ProductOfArrayExceptSelfPromise.IsKeptBy(nums));
}
