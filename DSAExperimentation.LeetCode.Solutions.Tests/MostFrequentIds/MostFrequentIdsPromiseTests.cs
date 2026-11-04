using DSAExperimentation.LeetCode.Solutions.MostFrequentIds;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MostFrequentIds;

public sealed partial class MostFrequentIdsPromiseTests
{
    public static TheoryData<int[], int[], bool> Inputs =>
        new()
        {
            { [2, 3, 2, 1], [3, 2, -3, 1], true },
            { [5, 5, 3], [2, -2, 1], true },
            { [1, 1], [-1, -1], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenNoCountGoesNegative(int[] nums, int[] freq, bool expected)
        => Assert.Equal(expected, MostFrequentIdsPromise.IsKeptBy(nums, freq));
}
