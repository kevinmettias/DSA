namespace DSAExperimentation.Domain.Language.English.NumberNames.Readings;

// English's readings of the symbols mathematics and computing add to the numbers: ∞ is "infinity"
// (the affine +∞ and the projective ∞ alike) and -∞ "negative infinity"; IEEE 754's NaN "not a
// number"; i, the imaginary unit, "i"; ω "omega"; ℵ0 "aleph-null", also said aleph-nought or
// aleph-zero. The concepts are Mathematics/ExtendedReals, Computing/FloatingPoint,
// Mathematics/NumberSystems and Mathematics/SetTheory's; English only names them.
/// <summary>
/// English's readings of infinity, NaN, the imaginary unit, omega and aleph-null.
/// </summary>
public static class EnglishMathematicalWords
{
    /// <summary>Gets "infinity", the affine +∞ and the projective ∞.</summary>
    public static string Infinity => "infinity";

    /// <summary>Gets "negative infinity", the affine -∞.</summary>
    public static string NegativeInfinity => "negative infinity";

    /// <summary>Gets "not a number", IEEE 754's NaN.</summary>
    public static string NotANumber => "not a number";

    /// <summary>Gets "i", the imaginary unit, a square root of -1.</summary>
    public static string ImaginaryUnit => "i";

    /// <summary>Gets "omega", the first infinite ordinal ω.</summary>
    public static string Omega => "omega";

    /// <summary>Gets "aleph-null", the smallest infinite cardinal ℵ0.</summary>
    public static string AlephNull => "aleph-null";
}
