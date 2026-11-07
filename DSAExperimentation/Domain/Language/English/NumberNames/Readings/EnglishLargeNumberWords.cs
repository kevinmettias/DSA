using System.Collections.Immutable;
using System.Numerics;
using DSAExperimentation.Domain.Mathematics.Numerals;

namespace DSAExperimentation.Domain.Language.English.NumberNames.Readings;

// English's names for large numbers that are not steps of a scale: a googol, 10^100, named by
// Milton Sirotta in 1920 and published by his uncle Edward Kasner in 1940, and a googolplex, 10 to
// the googol. A googolplex's exponent is a googol itself, so it is read from the googol rather than
// written again. No scale composes with them: "a googol thousand" is not how anyone reads 10^103.
/// <summary>
/// English's named large numbers that are not scale steps: googol and googolplex.
/// </summary>
public static class EnglishLargeNumberWords
{
    /// <summary>Gets a googol, ten to the hundred: 10^100.</summary>
    public static PowerOfTenWord Googol { get; } = new(EnglishNumberWords.Hundred.Value, "googol");

    /// <summary>Gets a googolplex, 10 to the googol.</summary>
    public static PowerOfTenWord Googolplex { get; } =
        new(BigInteger.Pow(DecimalDigits.Size, (int)Googol.Exponent), "googolplex");

    /// <summary>Gets the named large numbers in increasing order.</summary>
    public static ImmutableArray<PowerOfTenWord> All { get; } =
        [.. new[] { Googol, Googolplex }.OrderBy(word => word.Exponent)];
}
