namespace DSAExperimentation.Domain.Mathematics.Geometry;

// The flat plane's own angle sum: a triangle's three interior angles make a half turn, 180 degrees,
// whatever its size, as LC 3899 relies on. It is the flat case of ConstantCurvatureSurface's
// Gauss–Bonnet sum, written as a constant so a caller pays nothing for it (ARCHITECTURE §17.6).
/// <summary>Facts of the flat (Euclidean) plane.</summary>
public static class EuclideanPlane
{
    /// <summary>The degrees a triangle's three interior angles add up to on the flat plane.</summary>
    public const double TriangleAngleSumDegrees = 180.0;
}
