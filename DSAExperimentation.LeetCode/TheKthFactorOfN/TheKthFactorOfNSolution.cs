using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.LeetCode.SqrtX;

namespace DSAExperimentation.LeetCode.TheKthFactorOfN;

// LeetCode 1492. The kth Factor of n: report the kth smallest divisor of n, or -1
// when n has fewer than k divisors.
//
// Both strategies answer the same question and differ only in how much of the range
// they walk. The baseline trial-divides every candidate from 1 to n. The composed
// strategy anchors at floor(sqrt(n)) with one BinarySearch.LowerBound over SqrtX's
// monotone SquareExceedsSequence - LC 69's own "does i^2 exceed x" witness, reused
// rather than copied, the same technique FourDivisors and ClosestDivisors use - then
// counts divisors off in sorted order: ascending up to the anchor, and past it by
// walking the anchor back down to 1 and reporting each partner n/divisor (also
// ascending, since n/divisor grows as divisor shrinks).
//
// Both strategies take LeetCode's own two integers, so neither needs a hoisted
// prepared-input overload - there is no input structure to build.
internal static class TheKthFactorOfNSolution
{
    // ceil(sqrt(int.MaxValue)): caps the binary-search anchor so squaring an index can
    // never overflow the search range.
    private const int SqrtAnchorCeiling = 46_341;

    // The textbook answer: trial-divide n by every candidate from 1 upward, counting
    // factors off until the kth appears. Deliberately written without this repo's
    // primitives - it is the arm the composed strategy below has to justify itself
    // against.
    public static int KthFactorByFullRangeScan(int n, int k)
    {
        var remaining = k;

        for (var divisor = 1; divisor <= n; divisor++)
        {
            if (n % divisor == 0 && --remaining == 0)
            {
                return divisor;
            }
        }

        return LeetCodeAnswer.None;
    }

    // Anchor at floor(sqrt(n)) in O(log n), then walk the two halves of the divisor
    // list in sorted order and stop the moment the kth factor is found - never more
    // than O(sqrt(n)) divisibility tests instead of O(n).
    public static int KthFactorByBinarySearchAnchor(int n, int k)
    {
        var anchor = SqrtAnchor(n);
        var remaining = k;

        if (TryFindAtOrBelowAnchor(n, anchor, ref remaining, out var found))
        {
            return found;
        }

        return TryFindAboveAnchor(n, anchor, ref remaining, out found) ? found : LeetCodeAnswer.None;
    }

    // floor(sqrt(n)): the first index whose square exceeds n, less one.
    private static int SqrtAnchor(int n)
    {
        var sequence = new SquareExceedsSequence(n, Math.Min(n, SqrtAnchorCeiling) + 1);

        return BinarySearch.LowerBound<int, SquareExceedsSequence>(sequence, 1) - 1;
    }

    // The small half of the divisor list, already in ascending order.
    private static bool TryFindAtOrBelowAnchor(int n, int anchor, ref int remaining, out int found)
    {
        for (var divisor = 1; divisor <= anchor; divisor++)
        {
            if (n % divisor == 0 && --remaining == 0)
            {
                found = divisor;
                return true;
            }
        }

        found = LeetCodeAnswer.None;
        return false;
    }

    // The large half: walking the anchor back down to 1 and reporting n/divisor visits
    // the divisors above sqrt(n) in ascending order. A perfect square's root is its own
    // partner and was already counted above, so it is skipped here.
    private static bool TryFindAboveAnchor(int n, int anchor, ref int remaining, out int found)
    {
        for (var divisor = anchor; divisor >= 1; divisor--)
        {
            if (divisor * divisor == n || n % divisor != 0)
            {
                continue;
            }

            if (--remaining == 0)
            {
                found = n / divisor;
                return true;
            }
        }

        found = LeetCodeAnswer.None;
        return false;
    }
}
