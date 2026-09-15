using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.LeetCode.GreatestCommonDivisorOfStrings;

// LeetCode 1071. Greatest Common Divisor of Strings: the longest string that divides
// both str1 and str2, where "x divides s" means s is x repeated some number of times.
//
// Both strategies rest on the same fact - a common divisor exists exactly when
// str1 + str2 is one repeating block - and differ only in how they test it: the
// algebraic identity str1+str2 == str2+str1, or the shortest period of str1+str2 read
// straight off a failure function.
internal static class GreatestCommonDivisorOfStringsSolution
{
    // The textbook answer: a common divisor exists iff the two concatenations agree,
    // and its length is then the integer gcd of the two lengths. Deliberately BCL-only,
    // and it materializes both concatenations to compare them.
    public static string GcdOfStringsByConcatenationEquality(string str1, string str2)
    {
        if (str1 + str2 != str2 + str1)
        {
            return string.Empty;
        }

        var length = Gcd(str1.Length, str2.Length);

        return str1[..length];
    }

    // This repo's own PrefixFunctionSearch: the failure function's last entry gives
    // str1+str2's shortest period directly, and a common divisor exists exactly when
    // that period evenly divides both original lengths - the same repeating-period
    // question RepeatedSubstringPattern answers, applied to the concatenation, and
    // without ever building the swapped concatenation str2+str1.
    //
    // The period is the SMALLEST common divisor, not the greatest: LC asks for the
    // longest, which is the gcd-length prefix - always a whole number of periods, and
    // equal to the period itself exactly when the two repeat counts are coprime.
    public static string GcdOfStringsByPrefixFunctionPeriod(string str1, string str2)
    {
        var concatenated = str1 + str2;
        var failure = PrefixFunctionSearch.ComputeFailureFunction(concatenated);
        var period = concatenated.Length - failure[^1];

        if (str1.Length % period != 0 || str2.Length % period != 0)
        {
            return string.Empty;
        }

        return str1[..Gcd(str1.Length, str2.Length)];
    }

    private static int Gcd(int first, int second) => second == 0 ? first : Gcd(second, first % second);
}
