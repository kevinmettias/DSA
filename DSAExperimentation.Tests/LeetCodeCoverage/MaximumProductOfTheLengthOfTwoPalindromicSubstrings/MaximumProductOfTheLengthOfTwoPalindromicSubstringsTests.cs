using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumProductOfTheLengthOfTwoPalindromicSubstrings;

// LeetCode 1960. Maximum Product of the Length of Two Palindromic Substrings: this
// repo's own Manacher.ComputeOddRadii already gives, for every center, the radius of
// every odd-length palindrome nested there (radius k valid for every 1 <= k <=
// oddRadii[center], since a palindrome of radius r is trivially also a palindrome at
// every smaller radius sharing its center). Marking each nested radius's right edge
// (for the "best palindrome usable as a left piece ending at or before position i")
// and left edge (mirror, for the right piece) with its length, then taking a running
// prefix/suffix max, gives - for every split point - the longest odd palindrome fully
// contained on each side; the answer is the best product across every split. This is
// the same "read Manacher's per-center radius, not just the longest one" move
// PalindromicSubstringsTests already uses for LC 647.
public sealed partial class MaximumProductOfTheLengthOfTwoPalindromicSubstringsTests
{
    [Theory]
    [InlineData("ababbb", 9L)] // "aba" (0..2) * "bbb" (3..5)
    [InlineData("zaaaxbbby", 9L)] // "aaa" * "bbb"
    [InlineData("aaaaa", 3L)] // "aaa" (a nested sub-palindrome, not the whole string) * "a"
    public void MaxProduct_LeetCodeAndNestedPalindromeExamples_ReturnsBestSplitProduct(string s, long expected)
        => Assert.Equal(expected, MaxProduct(s));

    private static long MaxProduct(string s)
    {
        var oddRadii = Manacher.ComputeOddRadii(s);
        var leftBest = BestPalindromeLength(s.Length, oddRadii, fromLeft: true);
        var rightBest = BestPalindromeLength(s.Length, oddRadii, fromLeft: false);

        var best = 0L;

        for (var split = 0; split < s.Length - 1; split++)
        {
            best = Math.Max(best, (long)leftBest[split] * rightBest[split + 1]);
        }

        return best;
    }

    // fromLeft: best[i] = length of the longest odd palindrome entirely within
    // s[0..i]. Otherwise: best[i] = length of the longest odd palindrome entirely
    // within s[i..length-1]. Both directions read the same per-center radii from
    // Manacher and only differ in which edge of each nested palindrome they mark
    // and which way the running-max sweep travels.
    private static int[] BestPalindromeLength(int length, int[] oddRadii, bool fromLeft)
    {
        var best = new int[length];
        Array.Fill(best, 1);

        MarkNestedPalindromeEdges(best, oddRadii, fromLeft);
        SweepRunningMax(best, fromLeft);

        return best;
    }

    private static void MarkNestedPalindromeEdges(int[] best, int[] oddRadii, bool fromLeft)
    {
        for (var center = 0; center < best.Length; center++)
        {
            for (var radius = 1; radius <= oddRadii[center]; radius++)
            {
                var edge = fromLeft ? center + radius - 1 : center - radius + 1;
                best[edge] = Math.Max(best[edge], (2 * radius) - 1);
            }
        }
    }

    private static void SweepRunningMax(int[] best, bool fromLeft)
    {
        if (fromLeft)
        {
            for (var i = 1; i < best.Length; i++)
            {
                best[i] = Math.Max(best[i], best[i - 1]);
            }
        }
        else
        {
            for (var i = best.Length - 2; i >= 0; i--)
            {
                best[i] = Math.Max(best[i], best[i + 1]);
            }
        }
    }
}
