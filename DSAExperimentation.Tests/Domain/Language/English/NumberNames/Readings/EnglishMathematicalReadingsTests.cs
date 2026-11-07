using DSAExperimentation.Domain.Mathematics.ExtendedReals;
using DSAExperimentation.Domain.Mathematics.SetTheory;
using ShortScaleReadings = DSAExperimentation.Domain.Language.English.NumberNames.Readings.EnglishMathematicalReadings<
    DSAExperimentation.Domain.Language.English.NumberNames.Scales.ShortScale>;

namespace DSAExperimentation.Tests.Domain.Language.English.NumberNames.Readings;

public sealed partial class EnglishMathematicalReadingsTests
{
    public static TheoryData<long[], string> HypercomplexNumbers => new()
    {
        { [3, 4], "three plus four i" },
        { [3, -4], "three minus four i" },
        { [0, 1], "i" },
        { [0, -1], "minus i" },
        { [0, -4], "minus four i" },
        { [-3, 0], "minus three" },
        { [0, 0], "zero" },
        { [1, 1], "one plus i" },
        { [1, 2, 3, 4], "one plus two i plus three j plus four k" },
        { [0, 0, -1, 1], "minus j plus k" },
        { [21], "twenty-one" },
    };

    public static TheoryData<Ordinal, string> Ordinals => new()
    {
        { Ordinal.Finite(3), "three" },
        { Ordinal.Omega, "omega" },
        { Ordinal.Of(1, 1), "omega plus one" },
        { Ordinal.Of(2, 0), "omega times two" },
        { Ordinal.Of(2, 3), "omega times two plus three" },
    };

    public static TheoryData<Cardinal, string> Cardinals => new()
    {
        { Cardinal.Finite(7), "seven" },
        { Cardinal.AlephNull, "aleph-null" },
        { Cardinal.Continuum, "beth one" },
        { Cardinal.Beth(2), "beth two" },
    };

    [Theory]
    [MemberData(nameof(HypercomplexNumbers))]
    public void Hypercomplex_Coordinates_ReadsTheRealPartThenEachUnit(long[] coordinates, string reading) =>
        Assert.Equal(reading, ShortScaleReadings.Hypercomplex(coordinates));

    [Fact]
    public void Hypercomplex_OctonionCoordinates_Throws() =>
        Assert.Throws<ArgumentException>(() => ShortScaleReadings.Hypercomplex(new long[8]));

    [Theory]
    [MemberData(nameof(Ordinals))]
    public void Transfinite_Ordinal_ReadsOmegaTimesAPlusB(Ordinal ordinal, string reading) =>
        Assert.Equal(reading, ShortScaleReadings.Transfinite(ordinal));

    [Theory]
    [MemberData(nameof(Cardinals))]
    public void Transfinite_Cardinal_ReadsTheCountAlephNullOrABeth(Cardinal cardinal, string reading) =>
        Assert.Equal(reading, ShortScaleReadings.Transfinite(cardinal));

    [Fact]
    public void AffinelyExtended_ReadsBothEndsAndFiniteNumbers()
    {
        Assert.Equal("infinity", ShortScaleReadings.AffinelyExtended(AffinelyExtendedReal<long>.PositiveInfinity));
        Assert.Equal("negative infinity", ShortScaleReadings.AffinelyExtended(AffinelyExtendedReal<long>.NegativeInfinity));
        Assert.Equal("minus five", ShortScaleReadings.AffinelyExtended(new AffinelyExtendedReal<long>(-5)));
    }

    [Fact]
    public void ProjectivelyExtended_ReadsInfinityAndFiniteNumbers()
    {
        Assert.Equal("infinity", ShortScaleReadings.ProjectivelyExtended(ProjectivelyExtendedReal<long>.Infinity));
        Assert.Equal("twelve", ShortScaleReadings.ProjectivelyExtended(new ProjectivelyExtendedReal<long>(12)));
    }
}
