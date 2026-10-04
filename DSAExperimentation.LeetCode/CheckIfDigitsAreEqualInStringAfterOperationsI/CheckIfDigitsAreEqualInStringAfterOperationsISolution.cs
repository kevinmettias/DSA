using DSAExperimentation.Algorithms.NumberTheory;

namespace DSAExperimentation.LeetCode.CheckIfDigitsAreEqualInStringAfterOperationsI;

// LeetCode 3461. Check If Digits Are Equal in String After Operations I: repeatedly
// replace s with the length-(n-1) string of (s[i]+s[i+1]) % 10 for consecutive
// pairs, until exactly two digits remain, and report whether they're equal.
//
// s.Length is at most 100. The two strategies are genuinely different algorithms:
// direct repeated reduction over plain digit arrays, the textbook arm, versus
// reading the two final digits straight off row (n-2) of Pascal's triangle - a
// PascalTriangle row - since n-2 reduction steps is exactly what Pascal's
// triangle's addition rule computes.
internal static class CheckIfDigitsAreEqualInStringAfterOperationsISolution
{
    // Both strategies reduce modulo the same thing - a single decimal digit - and the
    // file states that base in two places, so it is named once.
    private const int DecimalDigitModulus = 10;

    public static bool IsEqualByAdjacentSumReduction(string digitString)
    {
        var digits = ToDigits(digitString);

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
    // the two digits IsEqualByAdjacentSumReduction ends with - computed here by
    // reading the coefficient row off a PascalTriangle once instead of materializing
    // every intermediate string. The triangle is built modulo 10: only the final
    // digit is ever read, Pascal's rule is a sum, and a sum's last digit depends only
    // on its terms' last digits. Built exactly, the row outgrows even a long past
    // row 66 (PascalTriangle.MaxExactRow), short of the 98 steps s.Length 100 needs.
    public static bool IsEqualByPascalRowCoefficients(string digitString)
    {
        var digits = ToDigits(digitString);
        var steps = digits.Length - 2;
        var row = PascalTriangle.Modulo(steps, DecimalDigitModulus).Row(steps);

        return Reduce(digits, row, 0) == Reduce(digits, row, 1);
    }

    private static long Reduce(int[] digits, ReadOnlySpan<long> coefficients, int offset)
    {
        var sum = 0L;

        for (var i = 0; i < coefficients.Length; i++)
        {
            sum += coefficients[i] * digits[offset + i];
        }

        return sum % DecimalDigitModulus;
    }

    private static int[] ToDigits(string digitString)
    {
        var digits = new int[digitString.Length];

        for (var i = 0; i < digitString.Length; i++)
        {
            digits[i] = digitString[i] - '0';
        }

        return digits;
    }
}
