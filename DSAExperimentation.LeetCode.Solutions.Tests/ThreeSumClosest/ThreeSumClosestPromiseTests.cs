using DSAExperimentation.LeetCode.Solutions.ThreeSumClosest;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ThreeSumClosest;

public sealed partial class ThreeSumClosestPromiseTests
{
    public static TheoryData<int[], int, bool> Inputs =>
        new()
        {
            { [-1, 2, 1, -4], 1, true },
            { [0, 0, 0], 1, true },
            { [1, 2, 3, 4], 6, true },
            { [1000, -1000, 500, -500, 250, -250, 125, -125], 0, false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenOneSumIsClosest(int[] nums, int target, bool expected)
        => Assert.Equal(expected, ThreeSumClosestPromise.IsKeptBy(nums, target));
}
