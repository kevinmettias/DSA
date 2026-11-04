using DSAExperimentation.LeetCode.Solutions.ArrayPartition;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ArrayPartition;

public sealed partial class ArrayPartitionPromiseTests
{
    public static TheoryData<int[], bool> Inputs =>
        new()
        {
            { [1, 4, 3, 2], true },
            { [1, 2, 3], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForAnEvenCount(int[] nums, bool expected)
        => Assert.Equal(expected, ArrayPartitionPromise.IsKeptBy(nums));
}
