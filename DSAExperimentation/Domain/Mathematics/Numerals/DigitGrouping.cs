using System.Globalization;
using System.Text;

namespace DSAExperimentation.Domain.Mathematics.Numerals;

// A decimal numeral written with its digits grouped, the groups counted from the right. The
// grouping's sizes are a struct witness's constants, so each preset compiles to its own numbers.
/// <summary>Writes a decimal numeral with its digits grouped by a grouping.</summary>
/// <typeparam name="TGrouping">The grouping, such as the thousands or the Indian.</typeparam>
public static class DigitGrouping<TGrouping>
    where TGrouping : struct, IDigitGrouping
{
    /// <summary>Writes a number's decimal digits with a separator between the groups.</summary>
    /// <param name="value">The number; a negative one is written with a leading minus sign.</param>
    /// <param name="separator">The character between two groups, such as a comma.</param>
    /// <returns>
    /// The grouped numeral, such as 1,00,00,000 for ten million in the Indian grouping.
    /// </returns>
    public static string Write(long value, char separator)
    {
        var digits = Magnitude(value).ToString(CultureInfo.InvariantCulture);
        var numeral = new StringBuilder(digits);
        var boundary = digits.Length - TGrouping.FirstGroupDigits;

        while (boundary > 0)
        {
            numeral.Insert(boundary, separator);
            boundary -= TGrouping.LaterGroupDigits;
        }

        if (value < 0)
        {
            numeral.Insert(0, CultureInfo.InvariantCulture.NumberFormat.NegativeSign);
        }

        return numeral.ToString();
    }

    // The number's size, long.MinValue's included, which has no long negative.
    private static ulong Magnitude(long value)
    {
        if (value < 0)
        {
            return (ulong)-(value + 1) + 1;
        }

        return (ulong)value;
    }
}
