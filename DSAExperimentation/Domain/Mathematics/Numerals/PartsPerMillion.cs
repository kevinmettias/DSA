namespace DSAExperimentation.Domain.Mathematics.Numerals;

// Parts per million: a proportion as millionths, as a trace concentration in air or water is given.
/// <summary>Parts per million: a proportion written as parts of 1,000,000.</summary>
public readonly struct PartsPerMillion : IPartsPerNotation
{
    /// <summary>The parts in a whole, 1,000,000.</summary>
    public const long PartsPerWhole = 1_000_000;

    static long IPartsPerNotation.PartsPerWhole => PartsPerWhole;
}
