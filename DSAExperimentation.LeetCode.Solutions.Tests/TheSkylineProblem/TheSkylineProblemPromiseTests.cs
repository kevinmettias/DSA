using DSAExperimentation.LeetCode.Solutions.TheSkylineProblem;

namespace DSAExperimentation.LeetCode.Solutions.Tests.TheSkylineProblem;

public sealed partial class TheSkylineProblemPromiseTests
{
    public static TheoryData<int[][], bool> Inputs =>
        new()
        {
            { [[2, 9, 10], [3, 7, 15], [5, 12, 12], [15, 20, 10], [19, 24, 8]], true },
            { [[0, 2, 3], [2, 5, 3]], true },
            { [[50, 50, 20]], false },
            { [[5, 9, 2], [1, 3, 4]], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForSortedBuildingsWithWidth(int[][] buildings, bool expected)
        => Assert.Equal(expected, TheSkylineProblemPromise.IsKeptBy(buildings));
}
