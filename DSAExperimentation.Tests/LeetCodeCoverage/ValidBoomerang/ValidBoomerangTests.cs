namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidBoomerang;

// LeetCode 1037. Valid Boomerang: three points form a boomerang exactly when they
// are not collinear, decided by a single O(1) 2D cross product over the two edge
// vectors from the first point. No repo container or algorithm primitive applies -
// there is nothing to compose over three fixed (x,y) pairs and one closed-form sign
// check, the same "lighter repo-primitive fit" case ComplexNumberMultiplication and
// MirrorReflection already establish.
public sealed partial class ValidBoomerangTests
{
    [Fact]
    public void IsBoomerang_NonCollinearPoints_ReturnsTrue()
    {
        int[][] points = [[1, 1], [2, 3], [3, 2]];

        Assert.True(IsBoomerang(points));
    }

    [Fact]
    public void IsBoomerang_CollinearPoints_ReturnsFalse()
    {
        int[][] points = [[1, 1], [2, 2], [3, 3]];

        Assert.False(IsBoomerang(points));
    }

    [Fact]
    public void IsBoomerang_DuplicatePoints_ReturnsFalse()
    {
        int[][] points = [[0, 0], [0, 0], [1, 1]];

        Assert.False(IsBoomerang(points));
    }

    private static bool IsBoomerang(int[][] points)
    {
        var (x1, y1) = (points[0][0], points[0][1]);
        var (x2, y2) = (points[1][0], points[1][1]);
        var (x3, y3) = (points[2][0], points[2][1]);

        var cross = ((long)(x2 - x1) * (y3 - y1)) - ((long)(x3 - x1) * (y2 - y1));
        return cross != 0;
    }
}
