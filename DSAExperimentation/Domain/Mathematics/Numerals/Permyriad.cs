namespace DSAExperimentation.Domain.Mathematics.Numerals;

// Per myriad, "for each ten thousand": a proportion as ten-thousandths, which finance calls basis
// points, so a rate rising 25 basis points rises a quarter of a per cent.
/// <summary>Per myriad, finance's basis point: a proportion written as parts of 10,000.</summary>
public readonly struct Permyriad : IPartsPerNotation
{
    /// <summary>The parts in a whole, 10,000.</summary>
    public const long PartsPerWhole = 10_000;

    static long IPartsPerNotation.PartsPerWhole => PartsPerWhole;
}
