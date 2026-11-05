using DSAExperimentation.Domain.Mathematics.Numerals;

namespace DSAExperimentation.Tests.Domain.Mathematics.Numerals;

// The whole is the fact itself, so the tests state each preset's, and read it both as the constant a
// caller names and through the notation a generic caller reads.
public sealed partial class PartsPerNotationPresetsTests
{
    public static TheoryData<long, long, long> Presets => new()
    {
        { 100, Percent.PartsPerWhole, WholeOf<Percent>() },
        { 1_000, Permille.PartsPerWhole, WholeOf<Permille>() },
        { 10_000, Permyriad.PartsPerWhole, WholeOf<Permyriad>() },
        { 1_000_000, PartsPerMillion.PartsPerWhole, WholeOf<PartsPerMillion>() },
    };

    [Theory]
    [MemberData(nameof(Presets))]
    public void PartsPerWhole_Preset_IsItsWholeAsConstantAndThroughTheNotation(
        long expected, long constant, long throughNotation)
    {
        Assert.Equal(expected, constant);
        Assert.Equal(expected, throughNotation);
    }

    private static long WholeOf<TNotation>()
        where TNotation : struct, IPartsPerNotation
        => TNotation.PartsPerWhole;
}
