namespace DSAExperimentation.Domain.Mathematics.Numerals;

// Per cent, "for each hundred": a proportion as hundredths, as LC 1357 states a discount and LC 2303 a
// tax rate. PartsPerWhole stays a constant for a caller that needs one; the notation reads it.
/// <summary>Per cent: a proportion written as parts of 100.</summary>
public readonly struct Percent : IPartsPerNotation
{
    /// <summary>The parts in a whole, 100.</summary>
    public const long PartsPerWhole = 100;

    static long IPartsPerNotation.PartsPerWhole => PartsPerWhole;
}
