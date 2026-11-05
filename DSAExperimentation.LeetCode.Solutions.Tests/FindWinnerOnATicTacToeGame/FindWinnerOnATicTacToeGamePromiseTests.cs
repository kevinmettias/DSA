using DSAExperimentation.LeetCode.Solutions.FindWinnerOnATicTacToeGame;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindWinnerOnATicTacToeGame;

public sealed partial class FindWinnerOnATicTacToeGamePromiseTests
{
    public static TheoryData<int[][], bool> Inputs =>
        new()
        {
            { [[0, 0], [2, 0], [1, 1], [2, 1], [2, 2]], true },
            { [[0, 0], [1, 1]], true },
            { [[0, 2], [1, 1], [0, 1], [0, 0], [1, 2], [2, 0], [2, 2], [2, 1]], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenNoMoveFollowsAWin(int[][] moves, bool expected)
        => Assert.Equal(expected, FindWinnerOnATicTacToeGamePromise.IsKeptBy(moves));
}
