namespace DSAExperimentation.Domain.Mathematics.Geometry;

// A surface of constant Gaussian curvature K: the flat plane (K = 0), a sphere of radius r
// (K = 1/r^2), or a hyperbolic plane (K < 0). On such a surface the Gauss–Bonnet theorem gives a
// geodesic triangle's angle sum as pi + K * Area, in radians: more than a half turn on a sphere, less
// on a hyperbolic plane, exactly one on the flat plane. This is the general form of the flat plane's
// 180 degrees, written only because it is exact (ARCHITECTURE §17.6).
/// <summary>A surface of constant curvature, and the angle sum of a triangle on it.</summary>
public readonly struct ConstantCurvatureSurface
{
    /// <summary>The flat plane, curvature zero.</summary>
    public static ConstantCurvatureSurface Flat => new(0.0);

    /// <summary>The surface's Gaussian curvature, the same at every point.</summary>
    public double Curvature { get; }

    private ConstantCurvatureSurface(double curvature) => Curvature = curvature;

    /// <summary>A sphere, whose curvature is one over its radius squared.</summary>
    /// <param name="radius">The sphere's radius; positive.</param>
    /// <returns>The sphere as a surface of curvature 1/radius².</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="radius"/> is not positive.</exception>
    public static ConstantCurvatureSurface Sphere(double radius)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(radius);

        return new ConstantCurvatureSurface(1.0 / (radius * radius));
    }

    /// <summary>A hyperbolic plane of the given negative curvature.</summary>
    /// <param name="curvature">The plane's curvature; negative.</param>
    /// <returns>The hyperbolic plane as a surface of that curvature.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="curvature"/> is not negative.</exception>
    public static ConstantCurvatureSurface HyperbolicPlane(double curvature)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(curvature, 0.0);

        return new ConstantCurvatureSurface(curvature);
    }

    /// <summary>The angle sum, in degrees, of a geodesic triangle of the given area on this surface.</summary>
    /// <param name="area">The triangle's area, in the units the curvature is measured in.</param>
    /// <returns>Gauss–Bonnet's pi + K·Area, converted from radians to degrees.</returns>
    public double TriangleAngleSumDegrees(double area) => (Math.PI + (Curvature * area)) * AngleUnits.DegreesPerRadian;
}
