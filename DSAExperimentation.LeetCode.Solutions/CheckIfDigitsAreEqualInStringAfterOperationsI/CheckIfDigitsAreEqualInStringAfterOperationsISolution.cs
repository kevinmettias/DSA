using DSAExperimentation.Domain.Mathematics.Numerals;

namespace DSAExperimentation.LeetCode.Solutions.CheckIfDigitsAreEqualInStringAfterOperationsI;

// LeetCode 3461. Check If Digits Are Equal in String After Operations I: repeatedly
// replace s with the length-(n-1) string of (s[i]+s[i+1]) % 10 for consecutive
// pairs, until exactly two digits remain, and report whether they're equal.
//
// s.Length is at most 100. The two strategies are genuinely different algorithms:
// direct repeated reduction over plain digit arrays, the textbook arm, versus
// reading the two final digits straight off row (n-2) of Pascal's triangle, since
// n-2 reduction steps is exactly what Pascal's triangle's addition rule computes.
// That one row is rolled in place rather than read off a PascalTriangle: the
// table keeps every row above the one wanted, O(n^2) memory where this needs
// O(n) - 43 KB against 848 bytes at s.Length 100 - which is the case the table's
// own complexity law sends to a rolling row.
internal static class CheckIfDigitsAreEqualInStringAfterOperationsISolution
{
    public static bool IsEqualByAdjacentSumReduction(string digitString)
    {
        var digits = ToDigits(digitString);

        while (digits.Length > 2)
        {
            var next = new int[digits.Length - 1];

            for (var i = 0; i < next.Length; i++)
            {
                next[i] = (digits[i] + digits[i + 1]) % DecimalDigits.Size;
            }

            digits = next;
        }

        return digits[0] == digits[1];
    }

    // Row k of Pascal's triangle gives the coefficients k reduction steps produce:
    // after k steps, the digit at position i is sum_j C(k, j) * original[i + j] mod
    // 10. With k = s.Length - 2, position 0 and position 1 of that row are exactly
    // the two digits IsEqualByAdjacentSumReduction ends with - computed here by
    // building the coefficient row once instead of materializing every
    // intermediate string. The row is built modulo 10: only the final digit is ever
    // read, Pascal's rule is a sum, and a sum's last digit depends only on its terms'
    // last digits. Built exactly, the row overflows an int from s.Length 36 on -
    // C(34, 17) is already past int.MaxValue - and the answer silently goes wrong.
    public static bool IsEqualByPascalRowCoefficients(string digitString)
    {
        var digits = ToDigits(digitString);
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
                coefficients[j] = (coefficients[j] + coefficients[j - 1]) % DecimalDigits.Size;
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

        return sum % DecimalDigits.Size;
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
