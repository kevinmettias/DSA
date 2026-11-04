using DSAExperimentation.LeetCode.Solutions.NumberOfWaysToReorderArrayToGetSameBST;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfWaysToReorderArrayToGetSameBST;

public sealed partial class NumberOfWaysToReorderArrayToGetSameBSTPromiseTests
{
    public static TheoryData<int[], bool> Inputs =>
        new()
        {
            { [2, 1, 3], true },
            { [3, 4, 5, 1, 2], true },
            { [2, 2, 3], false },
            { [1, 5], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForAPermutationOfOneToN(int[] nums, bool expected)
        => Assert.Equal(expected, NumberOfWaysToReorderArrayToGetSameBSTPromise.IsKeptBy(nums));
}
