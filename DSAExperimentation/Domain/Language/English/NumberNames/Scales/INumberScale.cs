using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Language.English.NumberNames.Scales;

// How English names a large number: its digits split into groups from the right, a first group,
// then groups of another size, each later group named by a word of the scale, and the number read
// group by group from the left. The short scale names every group of three (thousand, million,
// billion); the long scale reads a number below a million with "thousand" and names every group of
// six (million, billion, trillion, a billion being a million million); Indian English names the
// group of three and then every group of two (thousand, lakh, crore). The last word takes every
// digit above it, read again in the same scale, so 10^12 is one lakh crore in Indian English.
//
// The group sizes are static abstract members of a struct, read from the digit groupings of
// Mathematics/Numerals where a scale names the groups a numeral is written in, so they fold to the
// preset's numbers; each group word is a NumberWord whose number is 10 to the digits below it.
/// <summary>
/// A scale of English number names: the groups of digits it names, and their words.
/// </summary>
public interface INumberScale
{
    /// <summary>Gets the digits below the first group word.</summary>
    static abstract int FirstGroupDigits { get; }

    /// <summary>Gets the digits each later group word adds.</summary>
    static abstract int LaterGroupDigits { get; }

    /// <summary>Gets the word naming each group after the first, smallest first.</summary>
    static abstract ImmutableArray<NumberWord> GroupWords { get; }
}
