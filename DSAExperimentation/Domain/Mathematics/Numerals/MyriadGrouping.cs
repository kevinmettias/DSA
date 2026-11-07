namespace DSAExperimentation.Domain.Mathematics.Numerals;

// The East Asian grouping by myriads, four digits a group, as Chinese and Japanese number words
// name them: wan (10,000) and yi (100,000,000) each start a group, so a hundred million is
// 1,0000,0000.
/// <summary>Digits grouped by myriads: 4, then 4.</summary>
public readonly struct MyriadGrouping : IDigitGrouping
{
    private const int GroupDigits = 4;

    /// <inheritdoc/>
    public static int FirstGroupDigits => GroupDigits;

    /// <inheritdoc/>
    public static int LaterGroupDigits => GroupDigits;
}
