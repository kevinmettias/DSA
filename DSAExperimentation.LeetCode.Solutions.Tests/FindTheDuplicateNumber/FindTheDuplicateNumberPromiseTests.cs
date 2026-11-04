using DSAExperimentation.LeetCode.Solutions.FindTheDuplicateNumber;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindTheDuplicateNumber;

public sealed partial class FindTheDuplicateNumberPromiseTests
{
    public static TheoryData<int[], bool> Inputs =>
        new()
        {
            { [1, 3, 4, 2, 2], true },
            { [3, 3, 3, 3, 3], true },
            { [50000, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 50000], false },
            { [1, 2, 3], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenOneValueFromOneToNRepeats(int[] nums, bool expected)
        => Assert.Equal(expected, FindTheDuplicateNumberPromise.IsKeptBy(nums));
}
