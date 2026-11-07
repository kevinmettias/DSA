namespace DSAExperimentation.Domain.Mathematics.Numerals;

// The international grouping by thousands, three digits a group: 10,000,000. Each group of the
// short scale's names (thousand, million, billion) starts at one of these. The sizes are properties
// over a private constant, since a public constant of 3 would be read as the one source of every
// solution's own (SharedValueCopyTests).
/// <summary>Digits grouped by thousands: 3, then 3.</summary>
public readonly struct ThousandsGrouping : IDigitGrouping
{
    private const int GroupDigits = 3;

    /// <inheritdoc/>
    public static int FirstGroupDigits => GroupDigits;

    /// <inheritdoc/>
    public static int LaterGroupDigits => GroupDigits;
}
