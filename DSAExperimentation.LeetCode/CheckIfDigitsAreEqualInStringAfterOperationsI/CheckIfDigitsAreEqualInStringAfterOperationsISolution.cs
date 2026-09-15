namespace DSAExperimentation.LeetCode.CheckIfDigitsAreEqualInStringAfterOperationsI;

// LeetCode 3461. Check If Digits Are Equal in String After Operations I: repeatedly
// replace s with the length-(n-1) string of (s[i]+s[i+1]) % 10 for consecutive
// pairs, until exactly two digits remain, and report whether they're equal.
//
// s.Length is at most 10, so no repo primitive earns its place here - both
// strategies below are plain digit-array composition, the same "internals stay
// BCL" the naive arm of every migrated problem already keeps. The two strategies
// are genuinely different algorithms, not one primitive-composing and one
// textbook: direct repeated reduction versus reading the two final digits straight
// off row (n-2) of Pascal's triangle, since n-2 reduction steps is exactly what
// Pascal's triangle's addition rule computes.
internal static class CheckIfDigitsAreEqualInStringAfterOperationsISolution
{
    // Both strategies reduce modulo the same thing - a single decimal digit - and the
    // file states that base in two places, so it is named once.
    private const int DecimalDigitModulus = 10;

    public static bool AreEqualByAdjacentSumReduction(string s)
    {
        var digits = ToDigits(s);

        while (digits.Length > 2)
        {
            var next = new int[digits.Length - 1];

            for (var i = 0; i < next.Length; i++)
            {
                next[i] = (digits[i] + digits[i + 1]) % DecimalDigitModulus;
            }

            digits = next;
        }

        return digits[0] == digits[1];
    }

    // Row k of Pascal's triangle gives the coefficients k reduction steps produce:
    // after k steps, the digit at position i is sum_j C(k, j) * original[i + j] mod
    // 10. With k = s.Length - 2, position 0 and position 1 of that row are exactly
    // the two digits AreEqualByAdjacentSumReduction ends with - computed here by
    // building the coefficient row once instead of materializing every
    // intermediate string.
    public static bool AreEqualByPascalRowCoefficients(string s)
    {
        var digits = ToDigits(s);
        var steps = digits.Length - 2;
        var row = PascalRow(steps);

        return Reduce(digits, row, 0) == Reduce(digits, row, 1);
    }

    private static int[] PascalRow(int row)
    {
        var coefficients = new int[row + 1];
        coefficients[0] = 1;

        for (var i = 1; i <= row; i++)
        {
            for (var j = i; j > 0; j--)
            {
                coefficients[j] += coefficients[j - 1];
            }
        }

        return coefficients;
    }

    private static int Reduce(int[] digits, int[] coefficients, int offset)
    {
        var sum = 0;

        for (var i = 0; i < coefficients.Length; i++)
        {
            sum += coefficients[i] * digits[offset + i];
        }

        return sum % DecimalDigitModulus;
    }

    private static int[] ToDigits(string s)
    {
        var digits = new int[s.Length];

        for (var i = 0; i < s.Length; i++)
        {
            digits[i] = s[i] - '0';
        }

        return digits;
    }
}
