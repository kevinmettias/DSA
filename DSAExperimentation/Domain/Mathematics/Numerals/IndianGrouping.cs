namespace DSAExperimentation.Domain.Mathematics.Numerals;

// The Indian grouping: the first three digits, as the West groups them, then two digits a group, so
// ten million is written 1,00,00,000. A thousand, a lakh (1,00,000) and a crore (1,00,00,000) each
// start a group. The first group's three digits are the thousands grouping's.
/// <summary>Digits grouped as in India: 3, then 2.</summary>
public readonly struct IndianGrouping : IDigitGrouping
{
    private const int LaterDigits = 2;

    /// <inheritdoc/>
    public static int FirstGroupDigits => ThousandsGrouping.FirstGroupDigits;

    /// <inheritdoc/>
    public static int LaterGroupDigits => LaterDigits;
}
