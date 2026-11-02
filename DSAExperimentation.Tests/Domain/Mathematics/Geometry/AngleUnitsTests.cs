using DSAExperimentation.Domain.Mathematics.Geometry;

namespace DSAExperimentation.Tests.Domain.Mathematics.Geometry;

public sealed partial class AngleUnitsTests
{
    // The degrees in a turn define the degree, so the test states them.
    [Fact]
    public void FullTurnDegrees_IsThreeHundredSixty() => Assert.Equal(360.0, AngleUnits.FullTurnDegrees, precision: 9);

    // A full turn is 2 pi radians.
    [Fact]
    public void DegreesPerRadian_TwoPiRadians_IsAFullTurn() =>
        Assert.Equal(AngleUnits.FullTurnDegrees, 2 * Math.PI * AngleUnits.DegreesPerRadian, precision: 9);
}
