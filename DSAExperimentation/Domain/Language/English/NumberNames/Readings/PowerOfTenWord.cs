using System.Numerics;

namespace DSAExperimentation.Domain.Language.English.NumberNames.Readings;

// A word for a power of ten too large for a long to hold, given by its exponent: a googol is
// 10^100.
/// <summary>A word naming a power of ten, by its exponent.</summary>
/// <param name="Exponent">The power of ten the word names.</param>
/// <param name="Word">The word, lowercase.</param>
public readonly record struct PowerOfTenWord(BigInteger Exponent, string Word);
