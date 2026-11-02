using DSAExperimentation.Domain.Mathematics.Geometry;

namespace DSAExperimentation.Tests.Domain.Mathematics.Geometry;

// Known triangles on curved surfaces, each worked out by hand, check Gauss–Bonnet's pi + K * Area.
public sealed partial class ConstantCurvatureSurfaceTests
{
    public static TheoryData<double> Radii => new() { 1.0, 2.0, 6_371.0 };

    // The octant triangle, a pole and two equator points a quarter turn apart, has three right
    // angles, 270 degrees, and covers an eighth of the sphere, so the sum is the same at any radius.
    [Theory]
    [MemberData(nameof(Radii))]
    public void TriangleAngleSumDegrees_OctantOfASphere_IsThreeRightAngles(double radius)
    {
        var octantArea = 4 * Math.PI * radius * radius / 8;

        Assert.Equal(270.0, ConstantCurvatureSurface.Sphere(radius).TriangleAngleSumDegrees(octantArea), precision: 9);
    }

    // An ideal triangle, whose corners lie at infinity, has three zero angles and area pi / |K|.
    [Fact]
    public void TriangleAngleSumDegrees_IdealTriangleOnAHyperbolicPlane_IsZero() =>
        Assert.Equal(0.0, ConstantCurvatureSurface.HyperbolicPlane(-1.0).TriangleAngleSumDegrees(Math.PI), precision: 9);

    [Fact]
    public void Sphere_Radius_HasCurvatureOneOverItsSquare() =>
        Assert.Equal(0.25, ConstantCurvatureSurface.Sphere(2.0).Curvature, precision: 12);

    [Fact]
    public void Flat_HasZeroCurvature() => Assert.Equal(0.0, ConstantCurvatureSurface.Flat.Curvature, precision: 12);

    [Fact]
    public void Sphere_NonPositiveRadius_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => ConstantCurvatureSurface.Sphere(0.0));

    [Fact]
    public void HyperbolicPlane_NonNegativeCurvature_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => ConstantCurvatureSurface.HyperbolicPlane(0.0));
}
