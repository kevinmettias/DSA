using DSAExperimentation.Domain.Mathematics.Geometry;

namespace DSAExperimentation.Tests.Domain.Mathematics.Geometry;

public sealed partial class EuclideanPlaneTests
{
    [Fact]
    public void TriangleAngleSumDegrees_IsAHalfTurn() =>
        Assert.Equal(AngleUnits.FullTurnDegrees / 2, EuclideanPlane.TriangleAngleSumDegrees, precision: 9);

    // The flat plane's constant is the curved surfaces' Gauss–Bonnet sum at zero curvature, at any size.
    [Fact]
    public void TriangleAngleSumDegrees_IsTheFlatSurfacesSumAtAnyArea()
    {
        foreach (var area in new[] { 0.0, 1.0, 1_000.0 })
        {
            Assert.Equal(
                EuclideanPlane.TriangleAngleSumDegrees,
                ConstantCurvatureSurface.Flat.TriangleAngleSumDegrees(area),
                precision: 9);
        }
    }
}
