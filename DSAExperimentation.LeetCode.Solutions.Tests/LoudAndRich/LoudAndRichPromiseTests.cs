using DSAExperimentation.LeetCode.Solutions.LoudAndRich;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LoudAndRich;

public sealed partial class LoudAndRichPromiseTests
{
    public static TheoryData<int[][], int[], bool> Inputs =>
        new()
        {
            { [[1, 0], [2, 1], [3, 1], [3, 7], [4, 3], [5, 3], [6, 3]], [3, 2, 5, 4, 6, 1, 7, 0], true },
            { [], [0], true },
            { [[1, 0], [2, 1], [0, 2]], [3, 2, 1], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenTheObservationsFormNoCycle(int[][] richer, int[] quiet, bool expected)
        => Assert.Equal(expected, LoudAndRichPromise.IsKeptBy(richer, quiet));
}
