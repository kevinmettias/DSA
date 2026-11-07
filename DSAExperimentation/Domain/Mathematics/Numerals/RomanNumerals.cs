using System.Collections.Immutable;
using System.Text;

namespace DSAExperimentation.Domain.Mathematics.Numerals;

// The Roman numerals in their standard form, as written today. Seven symbols each stand for a
// number, I V X L C D M for 1, 5, 10, 50, 100, 500 and 1,000, and a numeral is their values added,
// largest first. A symbol repeats at most three times in a row, so 4 is not IIII but IV, one of six
// subtractive pairs: a power of ten, I, X or C, written before the next two symbols up, five and
// ten times it (IV, IX, XL, XC, CD, CM). So the largest numeral is MMMCMXCIX, 3,999, and there is
// none for zero. Older inscriptions wrote IIII and other forms, and larger numbers took a bar over
// a symbol for a thousand times it; neither is the standard form.
//
// Writing a number takes the largest symbol or pair that fits, over and over. Reading accepts only
// a numeral in standard form, so each number has one numeral and each numeral one number.
/// <summary>
/// The Roman numerals: their symbols, the subtractive pairs, and the numbers 1 to 3,999 written and
/// read.
/// </summary>
public static class RomanNumerals
{
    // A symbol repeats at most three times: III, but IV rather than IIII.
    private const int RepeatLimit = 3;

    // What a numeral is written from, largest first: each symbol and each pair with its value.
    // Built on first use, once the named values above it in the initialization order exist.
    private static readonly Lazy<(string Text, int Value)[]> Tokens = new(TokensLargestFirst);

    /// <summary>Gets I, one.</summary>
    public static RomanSymbol One { get; } = new('I', 1);

    /// <summary>Gets V, five.</summary>
    public static RomanSymbol Five { get; } = new('V', 5);

    /// <summary>Gets X, ten.</summary>
    public static RomanSymbol Ten { get; } = new('X', 10);

    /// <summary>Gets L, fifty.</summary>
    public static RomanSymbol Fifty { get; } = new('L', 50);

    /// <summary>Gets C, a hundred.</summary>
    public static RomanSymbol Hundred { get; } = new('C', 100);

    /// <summary>Gets D, five hundred.</summary>
    public static RomanSymbol FiveHundred { get; } = new('D', 500);

    /// <summary>Gets M, a thousand.</summary>
    public static RomanSymbol Thousand { get; } = new('M', 1_000);

    /// <summary>Gets IV, four.</summary>
    public static RomanPair Four { get; } = new(One, Five);

    /// <summary>Gets IX, nine.</summary>
    public static RomanPair Nine { get; } = new(One, Ten);

    /// <summary>Gets XL, forty.</summary>
    public static RomanPair Forty { get; } = new(Ten, Fifty);

    /// <summary>Gets XC, ninety.</summary>
    public static RomanPair Ninety { get; } = new(Ten, Hundred);

    /// <summary>Gets CD, four hundred.</summary>
    public static RomanPair FourHundred { get; } = new(Hundred, FiveHundred);

    /// <summary>Gets CM, nine hundred.</summary>
    public static RomanPair NineHundred { get; } = new(Hundred, Thousand);

    /// <summary>Gets the seven symbols in increasing order of value.</summary>
    public static ImmutableArray<RomanSymbol> Symbols { get; } =
        [
            .. new[] { One, Five, Ten, Fifty, Hundred, FiveHundred, Thousand }
                .OrderBy(symbol => symbol.Value),
        ];

    /// <summary>Gets the six subtractive pairs in increasing order of value.</summary>
    public static ImmutableArray<RomanPair> SubtractivePairs { get; } =
        [
            .. new[] { Four, Nine, Forty, Ninety, FourHundred, NineHundred }
                .OrderBy(pair => pair.Value),
        ];

    /// <summary>Gets the largest number with a standard numeral, MMMCMXCIX: 3,999.</summary>
    public static int Largest => (RepeatLimit * Thousand.Value) + Thousand.Value - One.Value;

    /// <summary>Writes a number as a Roman numeral in standard form.</summary>
    /// <param name="value">The number, 1 to 3,999.</param>
    /// <returns>The numeral, such as MCMXCIV for 1,994.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="value"/> is outside 1 to 3,999.
    /// </exception>
    public static string Write(int value)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(value, One.Value);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(value, Largest);

        var numeral = new StringBuilder();
        var remaining = value;

        foreach (var (text, tokenValue) in Tokens.Value)
        {
            while (remaining >= tokenValue)
            {
                numeral.Append(text);
                remaining -= tokenValue;
            }
        }

        return numeral.ToString();
    }

    private static (string Text, int Value)[] TokensLargestFirst() =>
    [
        .. Symbols.Select(symbol => (symbol.Letter.ToString(), symbol.Value))
            .Concat(SubtractivePairs.Select(pair => (pair.Text, pair.Value)))
            .OrderByDescending(token => token.Value),
    ];

    /// <summary>Reads a Roman numeral in standard form.</summary>
    /// <param name="numeral">The numeral, in capital letters.</param>
    /// <param name="value">
    /// The number it stands for; zero when it is not a standard numeral.
    /// </param>
    /// <returns>
    /// True for a numeral in standard form; false for an empty one, another letter, or a numeral
    /// such as IIII or IC that is not in standard form.
    /// </returns>
    public static bool TryRead(string numeral, out int value)
    {
        value = 0;

        if (!TrySumWithSubtraction(numeral, out var sum) || !IsStandardForm(numeral, sum))
        {
            return false;
        }

        value = sum;
        return true;
    }

    // The symbols' values added, each subtracted instead when a larger symbol follows it; false
    // when a letter is no symbol.
    private static bool TrySumWithSubtraction(string numeral, out int sum)
    {
        sum = 0;
        var next = 0;

        for (var at = numeral.Length - 1; at >= 0; at--)
        {
            var value = ValueOf(numeral[at]);

            if (value == 0)
            {
                return false;
            }

            sum += Signed(value, next);
            next = value;
        }

        return true;
    }

    // The symbol's value, or zero for a letter that is no symbol.
    private static int ValueOf(char letter)
    {
        foreach (var symbol in Symbols)
        {
            if (symbol.Letter == letter)
            {
                return symbol.Value;
            }
        }

        return 0;
    }

    // A symbol counts against the sum when a larger one follows it, as I does in IV.
    private static int Signed(int value, int next)
    {
        if (value < next)
        {
            return -value;
        }

        return value;
    }

    // A sum is the numeral's value only when writing that value gives the numeral back.
    private static bool IsStandardForm(string numeral, int sum)
    {
        if (sum < One.Value || sum > Largest)
        {
            return false;
        }

        return Write(sum) == numeral;
    }

}
