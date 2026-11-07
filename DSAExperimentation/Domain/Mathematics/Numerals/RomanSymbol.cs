namespace DSAExperimentation.Domain.Mathematics.Numerals;

// A Roman numeral's symbol and the number it stands for: X is 10.
/// <summary>A symbol of the Roman numerals and its value.</summary>
/// <param name="Letter">The symbol, a capital Latin letter.</param>
/// <param name="Value">The number it stands for.</param>
public readonly record struct RomanSymbol(char Letter, int Value);
