namespace DSAExperimentation.Tests.LeetCodeCoverage.CheckIfItIsAStraightLine;

// LeetCode 1232. Check If It Is a Straight Line: no repo primitive applies - pure
// O(n) integer cross-product scan over the input array (dx*(y-y0) == dy*(x-x0) for
// every point against the first two), the same "no stronger reusable primitive over
// a bare array" shape already established for GasStation/JumpGame/TrappingRainWater
// in this repo. Cross-multiplication avoids both floating-point slope division and
// a vertical-line (dx == 0) special case.
public sealed partial class CheckIfItIsAStraightLineTests
{
    [Fact]
    public void CheckStraightLine_CollinearPoints_ReturnsTrue()
    {
        int[][] coordinates = [[1, 2], [2, 3], [3, 4], [4, 5], [5, 6], [6, 7]];

        Assert.True(CheckStraightLine(coordinates));
    }

    [Fact]
    public void CheckStraightLine_OneOutlierPoint_ReturnsFalse()
    {
        int[][] coordinates = [[1, 1], [2, 2], [3, 4], [4, 5], [5, 6], [7, 7]];

        Assert.False(CheckStraightLine(coordinates));
    }

    [Fact]
    public void CheckStraightLine_VerticalLine_ReturnsTrue()
    {
        int[][] coordinates = [[3, 1], [3, 5], [3, -2]];

        Assert.True(CheckStraightLine(coordinates));
    }

    [Fact]
    public void CheckStraightLine_TwoPointsOnly_ReturnsTrue()
    {
        int[][] coordinates = [[0, 0], [1, 1]];

        Assert.True(CheckStraightLine(coordinates));
    }

    private static bool CheckStraightLine(int[][] coordinates)
    {
        var x0 = coordinates[0][0];
        var y0 = coordinates[0][1];
        var dx = coordinates[1][0] - x0;
        var dy = coordinates[1][1] - y0;

        for (var i = 2; i < coordinates.Length; i++)
        {
            var px = coordinates[i][0] - x0;
            var py = coordinates[i][1] - y0;

            if ((long)dx * py != (long)dy * px)
            {
                return false;
            }
        }

        return true;
    }
}
