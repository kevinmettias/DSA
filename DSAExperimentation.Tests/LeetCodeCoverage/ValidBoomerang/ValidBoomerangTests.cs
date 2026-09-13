using DSAExperimentation.LeetCode.ValidBoomerang;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidBoomerang;

// Harness only. Both strategies are ValidBoomerangSolution's - the floating-point
// Heron area and the exact integer cross product - pinned to the same examples so
// the tolerance-based arm is held to the exact one's answers, including the
// collinear and duplicate-point cases where its epsilon is doing the work.
public sealed class ValidBoomerangTests
{
    public static TheoryData<int[][], bool> Examples =>
        new()
        {
            // LC example 1.
            { [[1, 1], [2, 3], [3, 2]], true },

            // LC example 2: the three points sit on the line y = x.
            { [[1, 1], [2, 2], [3, 3]], false },

            // A repeated point degenerates the triangle to a segment.
            { [[0, 0], [0, 0], [1, 1]], false },

            // Collinear vertically, where the two edge vectors share a direction.
            { [[1, 1], [1, 2], [1, 3]], false },

            // Collinear horizontally, with the middle point given last.
            { [[0, 4], [8, 4], [3, 4]], false },

            // A boomerang whose points are given in clockwise order, so the cross
            // product is negative rather than positive.
            { [[0, 0], [1, 2], [2, 1]], true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsBoomerangByCrossProduct_LeetCodeExamples_ReturnsWhetherThePointsAreNonCollinear(
        int[][] points, bool expected) =>
        Assert.Equal(expected, ValidBoomerangSolution.IsBoomerangByCrossProduct(points));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsBoomerangByHeronArea_LeetCodeExamples_ReturnsWhetherThePointsAreNonCollinear(
        int[][] points, bool expected) =>
        Assert.Equal(expected, ValidBoomerangSolution.IsBoomerangByHeronArea(points));
}
