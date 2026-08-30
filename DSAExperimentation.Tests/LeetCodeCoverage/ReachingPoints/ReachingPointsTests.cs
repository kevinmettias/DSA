namespace DSAExperimentation.Tests.LeetCodeCoverage.ReachingPoints;

// LeetCode 780. Reaching Points: works backward from (tx, ty) toward (sx, sy),
// undoing whichever of (x+y, y)/(x, x+y) grew the larger coordinate - modulo
// instead of one subtraction at a time, the same jump-many-steps-at-once move
// Euclid's algorithm makes over repeated subtraction. No repo container or
// algorithm primitive applies here - there is nothing to compose over two
// running integer pairs, the same "lighter repo-primitive fit" case Pow(x, n)'s
// exponentiation by squaring already is.
public sealed partial class ReachingPointsTests
{
    [Theory]
    [InlineData(1, 1, 3, 5, true)]
    [InlineData(1, 1, 2, 2, false)]
    [InlineData(1, 1, 1, 1, true)]
    [InlineData(3, 5, 13, 5, true)]
    public void ReachingPoints_ClassicExamples_MatchesExpectedReachability(
        int sx, int sy, int tx, int ty, bool expected)
        => Assert.Equal(expected, IsReachable(sx, sy, tx, ty));

    private static bool IsReachable(int sx, int sy, int tx, int ty)
    {
        while (tx > sx && ty > sy)
        {
            if (tx > ty)
            {
                tx %= ty;
            }
            else
            {
                ty %= tx;
            }
        }

        if (tx == sx)
        {
            return ty >= sy && (ty - sy) % sx == 0;
        }

        if (ty == sy)
        {
            return tx >= sx && (tx - sx) % sy == 0;
        }

        return false;
    }
}
