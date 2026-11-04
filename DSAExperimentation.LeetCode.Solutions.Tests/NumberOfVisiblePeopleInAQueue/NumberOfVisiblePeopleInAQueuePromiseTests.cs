using DSAExperimentation.LeetCode.Solutions.NumberOfVisiblePeopleInAQueue;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfVisiblePeopleInAQueue;

public sealed partial class NumberOfVisiblePeopleInAQueuePromiseTests
{
    public static TheoryData<int[], bool> Inputs =>
        new()
        {
            { [10, 6, 8, 5, 11, 9], true },
            { [3, 1, 4, 1, 5], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenTheHeightsAreDistinct(int[] heights, bool expected)
        => Assert.Equal(expected, NumberOfVisiblePeopleInAQueuePromise.IsKeptBy(heights));
}
