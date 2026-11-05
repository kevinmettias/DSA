using DSAExperimentation.LeetCode.Solutions.ValidBoomerang;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ValidBoomerang;

// Harness only. Both strategies are ValidBoomerangSolution's - the floating-point
// Heron area and the exact integer cross product - pinned to the same cases so
// the tolerance-based arm is held to the exact one's answers, including the
// collinear and duplicate-point cases where its epsilon is doing the work.
public sealed partial class ValidBoomerangSolutionTests
{
    public static TheoryData<PointTripleCase> Cases =>
        new()
        {
            // A repeated point degenerates the triangle to a segment.
            new PointTripleCase([[0, 0], [0, 0], [1, 1]], IsBoomerang: false),

            // Collinear horizontally, with the middle point given last.
            new PointTripleCase([[0, 4], [8, 4], [3, 4]], IsBoomerang: false),
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsBoomerangByCrossProduct_Cases_ReturnsWhetherThePointsAreNonCollinear(
        PointTripleCase testCase) =>
        Assert.Equal(testCase.IsBoomerang, ValidBoomerangSolution.IsBoomerangByCrossProduct(testCase.Points));

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsBoomerangByHeronArea_Cases_ReturnsWhetherThePointsAreNonCollinear(
        PointTripleCase testCase) =>
        Assert.Equal(testCase.IsBoomerang, ValidBoomerangSolution.IsBoomerangByHeronArea(testCase.Points));

    // Nested because it is only ever used inside this test class and has no
    // independent identity: this harness's own vocabulary for one case.
    // The expected answer is a named field of the case rather than a bare `true` or
    // `false` sitting in the signature where only its position says what it means.
    public readonly record struct PointTripleCase(int[][] Points, bool IsBoomerang);
}
