using System.Text;

namespace DSAExperimentation.LeetCode.AmbiguousCoordinates;

// LeetCode 816. Ambiguous Coordinates: the original coordinate pair had its comma,
// spaces and decimal points stripped, leaving a digit run between parentheses.
// Report every "(x, y)" that could have produced it.
//
// Pure string generation over that digit run - split it into two non-empty halves
// at every position, then for each half either keep it whole or place a decimal
// point at every interior position, discarding an integer part with a leading zero
// (unless it is exactly "0") and a fractional part with a trailing zero. No repo
// primitive applies, the same category as GasStation/Candy/MaximumProductSubarray:
// a bounded combinatorial scan with no reusable structure to compose.
//
// Both strategies enumerate exactly the same candidates and differ only in how
// they validate one: the baseline materializes each dotted candidate with a
// StringBuilder and then re-scans it end to end (IndexOf the dot, re-walk both
// sides), while the composed strategy already knows the split point from the loop
// that produced it, so it slices directly and inspects only the two boundary
// characters that can violate the rule.
//
// The benchmark this migrated from had both arms *count* candidates rather than
// build them, to keep the measured return value cheap. LeetCode's actual answer is
// the list of coordinate strings, so both are promoted to return it here - the same
// deliberate change ARCHITECTURE.md 17.8 records for WordLadderII.
internal static class AmbiguousCoordinatesSolution
{
    // The string form of the number zero - the only integer part allowed to start
    // with a zero digit.
    private const string Zero = "0";

    private const char ZeroDigit = '0';
    private const char DecimalPoint = '.';

    // The baseline: rebuild every dotted candidate into a fresh string and validate
    // it by re-scanning the finished string. Deliberately written the way you would
    // without this repo - the arm the sliced strategy has to justify itself against.
    public static List<string> FindCoordinatesByRebuildAndRescan(string s)
    {
        var digits = DigitRun(s);

        return Combine(digits, RebuiltValidNumbers);
    }

    // Slice each half directly out of the digit run and check only the two boundary
    // characters - no rebuilt string, no re-scan.
    public static List<string> FindCoordinatesBySliceAndCheck(string s)
    {
        var digits = DigitRun(s);

        return Combine(digits, SlicedValidNumbers);
    }

    // Everything between the parentheses LeetCode wraps the input in.
    private static string DigitRun(string s) => s[1..^1];

    private static List<string> Combine(string digits, Func<string, IEnumerable<string>> validNumbers)
    {
        var results = new List<string>();

        for (var split = 1; split < digits.Length; split++)
        {
            var left = digits[..split];
            var right = digits[split..];

            foreach (var x in validNumbers(left))
            {
                foreach (var y in validNumbers(right))
                {
                    results.Add($"({x}, {y})");
                }
            }
        }

        return results;
    }

    private static IEnumerable<string> RebuiltValidNumbers(string digits)
    {
        if (IsValidWhole(digits))
        {
            yield return digits;
        }

        for (var dot = 1; dot < digits.Length; dot++)
        {
            var builder = new StringBuilder(digits.Length + 1);
            builder.Append(digits, 0, dot).Append(DecimalPoint).Append(digits, dot, digits.Length - dot);
            var candidate = builder.ToString();

            if (IsValidWithDot(candidate))
            {
                yield return candidate;
            }
        }
    }

    private static bool IsValidWhole(string digits) => digits == Zero || digits[0] != ZeroDigit;

    private static bool IsValidWithDot(string candidate)
    {
        var dotIndex = candidate.IndexOf(DecimalPoint);
        var intPart = candidate[..dotIndex];
        var fracPart = candidate[(dotIndex + 1)..];

        return IsValidWhole(intPart) && fracPart[^1] != ZeroDigit;
    }

    private static IEnumerable<string> SlicedValidNumbers(string digits)
    {
        if (IsValidWhole(digits))
        {
            yield return digits;
        }

        for (var dot = 1; dot < digits.Length; dot++)
        {
            var intPart = digits[..dot];
            var fracPart = digits[dot..];

            if (IsValidWhole(intPart) && fracPart[^1] != ZeroDigit)
            {
                yield return $"{intPart}{DecimalPoint}{fracPart}";
            }
        }
    }
}
