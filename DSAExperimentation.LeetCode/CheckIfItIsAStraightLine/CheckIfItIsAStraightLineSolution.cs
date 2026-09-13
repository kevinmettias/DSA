namespace DSAExperimentation.LeetCode.CheckIfItIsAStraightLine;

// LeetCode 1232. Check If It Is a Straight Line: decide whether every point in
// coordinates lies on one line. No repo primitive applies - this is plain integer
// arithmetic over a bare array, the same shape already established for GasStation,
// JumpGame and TrappingRainWater.
//
// Both strategies compare directions by cross-multiplication rather than slope
// division, which avoids floating point and removes the vertical-line (dx == 0)
// special case. They differ only in how much of the point set each one anchors
// against: CheckStraightLineByBruteForceEveryTriple is the textbook O(n^3) triple
// scan - the baseline, previously untested because it lived only in a benchmark -
// while CheckStraightLineByAnchoredCrossProductScan uses the fact that collinearity
// is transitive through a fixed anchor pair to settle it in one O(n) pass.
internal static class CheckIfItIsAStraightLineSolution
{
    // Index of the third point onward, once the first two points have anchored the
    // baseline direction vector.
    private const int ThirdPointIndex = 2;

    // Baseline: test every unordered triple (i, j, k) for collinearity. Deliberately
    // plain BCL arithmetic over the input array - what you would write without this
    // repo.
    public static bool CheckStraightLineByBruteForceEveryTriple(int[][] coordinates)
    {
        for (var i = 0; i < coordinates.Length; i++)
        {
            for (var j = i + 1; j < coordinates.Length; j++)
            {
                if (HasNonCollinearThirdPoint(coordinates, i, j))
                {
                    return false;
                }
            }
        }

        return true;
    }

    // One pass: every point must share a direction with the vector from the first
    // point to the second.
    public static bool CheckStraightLineByAnchoredCrossProductScan(int[][] coordinates)
    {
        var x0 = coordinates[0][0];
        var y0 = coordinates[0][1];
        var dx = (long)(coordinates[1][0] - x0);
        var dy = (long)(coordinates[1][1] - y0);

        for (var i = ThirdPointIndex; i < coordinates.Length; i++)
        {
            var px = (long)(coordinates[i][0] - x0);
            var py = (long)(coordinates[i][1] - y0);

            if (dx * py != dy * px)
            {
                return false;
            }
        }

        return true;
    }

    private static bool HasNonCollinearThirdPoint(int[][] coordinates, int i, int j)
    {
        for (var k = j + 1; k < coordinates.Length; k++)
        {
            var dx1 = (long)(coordinates[j][0] - coordinates[i][0]);
            var dy1 = (long)(coordinates[j][1] - coordinates[i][1]);
            var dx2 = (long)(coordinates[k][0] - coordinates[i][0]);
            var dy2 = (long)(coordinates[k][1] - coordinates[i][1]);

            if (dx1 * dy2 != dy1 * dx2)
            {
                return true;
            }
        }

        return false;
    }
}
