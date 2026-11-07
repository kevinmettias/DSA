namespace DSAExperimentation.Domain.Mathematics.Geometry;

// Angles in degrees, which LC 1610 measures with: a degree is defined as a 360th of a full turn, so
// a turn is 360 degrees and a radian is 180/pi of them. These are units, true on any surface. That
// the angles around a point make a full turn is a further fact that holds only where the surface is
// flat at that point (ARCHITECTURE §17.6). Every arm reads these, LC 1610's and LC 3899's
// baselines among them (§17.5); how a strategy wraps its angles around the 360/0 seam stays with
// it.
/// <summary>Angles measured in degrees.</summary>
public static class AngleUnits
{
    /// <summary>The degrees in one full turn, which defines the degree.</summary>
    public const double FullTurnDegrees = 360.0;

    /// <summary>The degrees in a half turn, a straight angle.</summary>
    public const double HalfTurnDegrees = FullTurnDegrees / 2;

    /// <summary>The degrees in one radian: a half turn is pi radians.</summary>
    public const double DegreesPerRadian = HalfTurnDegrees / Math.PI;
}
