namespace DSAExperimentation.Domain.Mathematics.Numerals;

// Per mille, "for each thousand": a proportion as thousandths, as a blood alcohol level or a slope is
// often given.
/// <summary>Per mille: a proportion written as parts of 1,000.</summary>
public readonly struct Permille : IPartsPerNotation
{
    /// <summary>The parts in a whole, 1,000.</summary>
    public const long PartsPerWhole = 1_000;

    static long IPartsPerNotation.PartsPerWhole => PartsPerWhole;
}
