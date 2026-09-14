using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.LeetCode.SumOfScoresOfBuiltStrings;

// LeetCode 2223. Sum of Scores of Built Strings: s is built one character at a
// time by PREPENDING, so the string after i steps - t_i - is s's length-i suffix,
// and score(t_i) is the length of the longest common prefix between t_i and the
// finished s. The answer is score(t_1) + ... + score(t_n).
//
// Written as a suffix-versus-whole-string comparison the total is O(n^2). The
// Z-array collapses it: z[j] is by definition the longest common prefix of s and
// s[j..], and t_i starts at index n - i, so score(t_i) IS z[n - i]. Summing z over
// every start index therefore sums every score, in one O(n) pass.
//
// A CORRECTION MADE IN MIGRATION. The pre-migration test summed the Z-array of the
// REVERSED string; the pre-migration benchmark's baseline compared suffixes
// directly. They are not the same function. z over reverse(s) measures the longest
// common SUFFIX of each PREFIX of s with s - a genuinely different quantity that
// happens to agree on palindromic or self-overlap-free inputs, which is every case
// the old test asserted ("babab", "azbazbzaz", "a", "abcde"). It disagrees on
// "aab" (reports 3, the answer is 4) and on "banana" (reports 10, the answer is
// 6). The suffix comparison was right, so the Z strategy below runs over s itself
// and both of those inputs are now asserted.
internal static class SumOfScoresOfBuiltStringsSolution
{
    // The textbook answer: for every suffix, walk it against s from the front
    // until the characters stop agreeing. Deliberately BCL-only - a pair of
    // indices over the string it is handed - since it is the arm the linear
    // strategy below has to justify itself against. Worst case O(n^2), reached
    // exactly when the string overlaps itself heavily.
    public static long SumScoresBySuffixComparison(string s)
    {
        var n = s.Length;
        var total = (long)n;

        for (var length = 1; length < n; length++)
        {
            var start = n - length;
            var score = 0;

            while (score < length && s[start + score] == s[score])
            {
                score++;
            }

            total += score;
        }

        return total;
    }

    // This repo's own ZFunction.Compute, which is already "longest common prefix
    // of s and s[j..], for every j" - precisely the per-suffix score this problem
    // asks to total, so the whole solution is one sweep of the array it returns.
    //
    // z[0] is left at zero rather than n by that type's own documented choice, so
    // the full-length score - t_n is s itself, and trivially shares all n
    // characters - is added separately instead of read out of the array. The
    // running total is a long because n can reach 10^5 and every score can be
    // O(n), which overflows an int.
    public static long SumScoresByZFunction(string s)
    {
        var z = ZFunction.Compute(s);
        var total = (long)s.Length;

        for (var start = 1; start < z.Length; start++)
        {
            total += z[start];
        }

        return total;
    }
}
