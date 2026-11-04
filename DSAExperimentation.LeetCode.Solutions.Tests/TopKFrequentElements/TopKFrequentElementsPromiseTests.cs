using DSAExperimentation.LeetCode.Solutions.TopKFrequentElements;

namespace DSAExperimentation.LeetCode.Solutions.Tests.TopKFrequentElements;

public sealed partial class TopKFrequentElementsPromiseTests
{
    public static TheoryData<int[], int, bool> Inputs =>
        new()
        {
            { [1, 1, 1, 2, 2, 3], 2, true },
            { [1], 1, true },
            { [1, 2, 1, 2, 1, 2, 3, 1, 3, 2], 1, false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenNoTieFallsAtTheKthFrequency(int[] nums, int topCount, bool expected)
        => Assert.Equal(expected, TopKFrequentElementsPromise.IsKeptBy(nums, topCount));
}
