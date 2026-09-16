using DSAExperimentation.LeetCode.ValidBoomerang;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidBoomerang;

// Harness only. Both strategies are ValidBoomerangSolution's - the floating-point
// Heron area and the exact integer cross product - pinned to the same examples so
// the tolerance-based arm is held to the exact one's answers, including the
// collinear and duplicate-point cases where its epsilon is doing the work.
public sealed partial class ValidBoomerangTests
{
    public static TheoryData<PointTripleExample> Examples =>
        new()
        {
            // LC example 1.
            new PointTripleExample([[1, 1], [2, 3], [3, 2]], IsBoomerang: true),

            // LC example 2: the three points sit on the line y = x.
            new PointTripleExample([[1, 1], [2, 2], [3, 3]], IsBoomerang: false),

            // A repeated point degenerates the triangle to a segment.
            new PointTripleExample([[0, 0], [0, 0], [1, 1]], IsBoomerang: false),

            // Collinear vertically, where the two edge vectors share a direction.
            new PointTripleExample([[1, 1], [1, 2], [1, 3]], IsBoomerang: false),

            // Collinear horizontally, with the middle point given last.
            new PointTripleExample([[0, 4], [8, 4], [3, 4]], IsBoomerang: false),

            // A boomerang whose points are given in clockwise order, so the cross
            // product is negative rather than positive.
            new PointTripleExample([[0, 0], [1, 2], [2, 1]], IsBoomerang: true),
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsBoomerangByCrossProduct_LeetCodeExamples_ReturnsWhetherThePointsAreNonCollinear(
        PointTripleExample example) =>
        Assert.Equal(example.IsBoomerang, ValidBoomerangSolution.IsBoomerangByCrossProduct(example.Points));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsBoomerangByHeronArea_LeetCodeExamples_ReturnsWhetherThePointsAreNonCollinear(
        PointTripleExample example) =>
        Assert.Equal(example.IsBoomerang, ValidBoomerangSolution.IsBoomerangByHeronArea(example.Points));

    // Nested because it is only ever used inside this test class and has no
    // independent identity: this harness's own vocabulary for one LeetCode example.
    // The expected answer is a named field of the case rather than a bare `true` or
    // `false` sitting in the signature where only its position says what it means.
    public readonly record struct PointTripleExample(int[][] Points, bool IsBoomerang);
}
