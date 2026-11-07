namespace DSAExperimentation.Domain.Mathematics.Numerals;

// A subtractive pair of the Roman numerals: a smaller symbol written before a larger one stands for
// their difference, so IV is 4 and CM is 900.
/// <summary>
/// A subtractive pair of Roman numerals: a symbol subtracted from the one it precedes.
/// </summary>
/// <param name="Subtracted">The smaller symbol, written first.</param>
/// <param name="From">The larger symbol, written second.</param>
public readonly record struct RomanPair(RomanSymbol Subtracted, RomanSymbol From)
{
    /// <summary>
    /// Gets the number the pair stands for, the larger symbol's value less the smaller's.
    /// </summary>
    public int Value => From.Value - Subtracted.Value;

    /// <summary>Gets the pair as written, the smaller symbol first.</summary>
    public string Text => string.Concat(Subtracted.Letter, From.Letter);
}
