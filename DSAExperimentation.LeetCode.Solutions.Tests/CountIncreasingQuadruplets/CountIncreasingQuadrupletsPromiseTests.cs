using DSAExperimentation.LeetCode.Solutions.CountIncreasingQuadruplets;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountIncreasingQuadruplets;

public sealed partial class CountIncreasingQuadrupletsPromiseTests
{
    public static TheoryData<int[], bool> Inputs =>
        new()
        {
            { [1, 3, 2, 4, 5], true },
            { [1, 2, 3, 4], true },
            { [1, 2, 2, 4], false },
            { [1, 2, 3, 9], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenNumsIsAPermutationOfOneToN(int[] nums, bool expected)
        => Assert.Equal(expected, CountIncreasingQuadrupletsPromise.IsKeptBy(nums));
}
