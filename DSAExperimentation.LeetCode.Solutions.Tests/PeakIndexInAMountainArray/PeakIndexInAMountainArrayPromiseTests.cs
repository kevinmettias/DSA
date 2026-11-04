using DSAExperimentation.LeetCode.Solutions.PeakIndexInAMountainArray;

namespace DSAExperimentation.LeetCode.Solutions.Tests.PeakIndexInAMountainArray;

public sealed partial class PeakIndexInAMountainArrayPromiseTests
{
    public static TheoryData<int[], bool> Inputs =>
        new()
        {
            { [0, 1, 0], true },
            { [0, 10, 5, 2], true },
            { [1, 2, 3], false },
            { [0, 2, 1, 2, 0], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForAMountain(int[] mountain, bool expected)
        => Assert.Equal(expected, PeakIndexInAMountainArrayPromise.IsKeptBy(mountain));
}
