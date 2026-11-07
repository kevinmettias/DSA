namespace DSAExperimentation.Domain.Mathematics.Numerals;

// How a numeral's digits are grouped for reading, counted from the right: a first group, then
// groups of another size up to the leading digits. The West groups by thousands, 3 then 3
// (10,000,000); India groups by 3 and then by hundreds, 3 then 2 (1,00,00,000), so a lakh and a
// crore each start a group; China and Japan group by myriads, 4 then 4 (1000,0000). Which character
// separates the groups, a comma, a point or a space, is a locale's choice and the caller's. The
// sizes are static abstract members of a struct, so a caller's arithmetic folds to the preset's
// numbers.
/// <summary>A grouping of a numeral's digits: a first group, then groups of another size.</summary>
public interface IDigitGrouping
{
    /// <summary>Gets the digits in the rightmost group.</summary>
    static abstract int FirstGroupDigits { get; }

    /// <summary>Gets the digits in each group to the left of the first.</summary>
    static abstract int LaterGroupDigits { get; }
}
