namespace DSAExperimentation.Tests.LeetCodeCoverage.AmbiguousCoordinates;

// LeetCode 816. Ambiguous Coordinates: pure string generation/validation over the
// digit run between the parentheses - split it into two non-empty halves at every
// position, then for each half either keep it as a whole integer or insert a
// decimal point at every position, discarding leading-zero integer parts (unless
// exactly "0") and trailing-zero fractional parts. No repo primitive applies - the
// same category already established for GasStation/Candy/MaximumProductSubarray:
// a bounded combinatorial scan over an array/string with no reusable data
// structure to compose.
public sealed partial class AmbiguousCoordinatesTests
{
    [Fact]
    public void AmbiguousCoordinates_ThreeDigits_ReturnsAllFourPlacements()
    {
        var results = FindAmbiguousCoordinates("(123)");

        Assert.Equal(
            new HashSet<string> { "(1, 23)", "(12, 3)", "(1.2, 3)", "(1, 2.3)" },
            new HashSet<string>(results));
    }

    [Fact]
    public void AmbiguousCoordinates_LeadingZeroDigit_DropsInvalidLeadingZeroPlacements()
    {
        var results = FindAmbiguousCoordinates("(0123)");

        Assert.Equal(
            new HashSet<string>
            {
                "(0, 123)", "(0, 12.3)", "(0, 1.23)", "(0.1, 23)", "(0.1, 2.3)", "(0.12, 3)",
            },
            new HashSet<string>(results));
    }

    private static List<string> FindAmbiguousCoordinates(string s)
    {
        var digits = s[1..^1];
        var results = new List<string>();

        for (var split = 1; split < digits.Length; split++)
        {
            var left = digits[..split];
            var right = digits[split..];

            foreach (var l in ValidNumbers(left))
            {
                foreach (var r in ValidNumbers(right))
                {
                    results.Add($"({l}, {r})");
                }
            }
        }

        return results;
    }

    private static IEnumerable<string> ValidNumbers(string digits)
    {
        if (digits == "0" || digits[0] != '0')
        {
            yield return digits;
        }

        for (var dot = 1; dot < digits.Length; dot++)
        {
            var intPart = digits[..dot];
            var fracPart = digits[dot..];

            if ((intPart == "0" || intPart[0] != '0') && fracPart[^1] != '0')
            {
                yield return $"{intPart}.{fracPart}";
            }
        }
    }
}
