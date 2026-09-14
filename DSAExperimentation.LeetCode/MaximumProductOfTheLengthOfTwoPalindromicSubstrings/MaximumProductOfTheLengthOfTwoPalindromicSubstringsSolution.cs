using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.LeetCode.MaximumProductOfTheLengthOfTwoPalindromicSubstrings;

// LeetCode 1960. Maximum Product of the Length of Two Palindromic Substrings: pick
// two non-intersecting odd-length palindromic substrings and maximize the product of
// their lengths. Every such pair is separated by some split point, so the answer is
// the best product of "longest odd palindrome entirely left of the split" and
// "longest odd palindrome entirely right of it" over every split.
//
// The baseline re-derives both sides from nothing at every split with the textbook
// expand-around-every-center walk - O(n) centers * O(n) expansion * O(n) splits.
// The composed strategy runs this repo's own O(n) Manacher.ComputeOddRadii once:
// radius k at a center means every radius 1 <= r <= k is also a palindrome there
// (a palindrome of radius k trivially contains the smaller ones sharing its center),
// so marking each nested radius's right edge - and, mirrored, its left edge - with
// that palindrome's length and then taking a running prefix/suffix max gives both
// per-split bests in one forward and one backward sweep. This is the same "read
// Manacher's per-center radius, not just the longest one" move
// PalindromicSubstringsSolution makes for LC 647.
//
// Both strategies take LeetCode's own string, so neither needs a hoisted
// prepared-input overload - there is no input structure to build.
internal static class MaximumProductOfTheLengthOfTwoPalindromicSubstringsSolution
{
    // Converts a Manacher-style radius count into the palindrome diameter it spans.
    private const int RadiusToDiameterMultiplier = 2;

    // The backward sweep starts one index before the last (already-seeded) slot.
    private const int BackwardSweepStartOffset = 2;

    // The textbook answer: for every split, expand around every center on each side
    // to find that side's longest odd palindrome, with no precomputation carried
    // between splits. Plain BCL indexing and nothing else - this is the arm the
    // composed strategy has to justify itself against.
    public static long MaxProductByCenterExpansion(string s)
    {
        var best = 0L;

        for (var split = 0; split < s.Length - 1; split++)
        {
            var leftBest = LongestOddPalindromeInRange(s, 0, split);
            var rightBest = LongestOddPalindromeInRange(s, split + 1, s.Length - 1);
            best = Math.Max(best, (long)leftBest * rightBest);
        }

        return best;
    }

    // Longest odd-length palindrome lying entirely within s[lo..hi], found by
    // expanding outward from every center in that window until it would leave it.
    private static int LongestOddPalindromeInRange(string s, int lo, int hi)
    {
        var best = 1;

        for (var center = lo; center <= hi; center++)
        {
            var radius = 0;

            while (center - radius - 1 >= lo && center + radius + 1 <= hi
                && s[center - radius - 1] == s[center + radius + 1])
            {
                radius++;
            }

            best = Math.Max(best, (RadiusToDiameterMultiplier * radius) + 1);
        }

        return best;
    }

    // One O(n) Manacher pass, then two sweeps over its per-center radii.
    public static long MaxProductByManacherRadii(string s)
    {
        var oddRadii = Manacher.ComputeOddRadii(s);
        var leftBest = BestPalindromeEndingAtOrBefore(s.Length, oddRadii);
        var rightBest = BestPalindromeStartingAtOrAfter(s.Length, oddRadii);

        var best = 0L;

        for (var split = 0; split < s.Length - 1; split++)
        {
            best = Math.Max(best, (long)leftBest[split] * rightBest[split + 1]);
        }

        return best;
    }

    // best[i] = length of the longest odd palindrome entirely within s[0..i].
    private static int[] BestPalindromeEndingAtOrBefore(int length, int[] oddRadii) =>
        BestPalindromeInDirection(length, oddRadii, forward: true);

    // best[i] = length of the longest odd palindrome entirely within s[i..length-1].
    private static int[] BestPalindromeStartingAtOrAfter(int length, int[] oddRadii) =>
        BestPalindromeInDirection(length, oddRadii, forward: false);

    // The two directions read the same per-center radii and differ only in which
    // edge of each nested palindrome they mark and which way the running max sweeps.
    private static int[] BestPalindromeInDirection(int length, int[] oddRadii, bool forward)
    {
        var best = InitializeBest(length);
        FillFromRadii(best, oddRadii, forward);

        if (forward)
        {
            SweepForward(best);
        }
        else
        {
            SweepBackward(best);
        }

        return best;
    }

    // Every position is its own length-1 odd palindrome, so that is the floor.
    private static int[] InitializeBest(int length)
    {
        var best = new int[length];
        Array.Fill(best, 1);
        return best;
    }

    private static void FillFromRadii(int[] best, int[] oddRadii, bool forward)
    {
        for (var center = 0; center < best.Length; center++)
        {
            for (var radius = 1; radius <= oddRadii[center]; radius++)
            {
                var edge = forward ? center + radius - 1 : center - radius + 1;
                best[edge] = Math.Max(best[edge], (RadiusToDiameterMultiplier * radius) - 1);
            }
        }
    }

    private static void SweepForward(int[] best)
    {
        for (var i = 1; i < best.Length; i++)
        {
            best[i] = Math.Max(best[i], best[i - 1]);
        }
    }

    private static void SweepBackward(int[] best)
    {
        for (var i = best.Length - BackwardSweepStartOffset; i >= 0; i--)
        {
            best[i] = Math.Max(best[i], best[i + 1]);
        }
    }
}
