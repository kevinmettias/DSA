using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.LeetCode.SqrtX;

namespace DSAExperimentation.LeetCode.TheKthFactorOfN;

// LeetCode 1492. The kth Factor of n: report the kth smallest divisor of `number`,
// or -1 when `number` has fewer than `rank` divisors.
//
// Both strategies answer the same question and differ only in how much of the range
// they walk. The baseline trial-divides every candidate from 1 to `number`. The
// composed strategy anchors at floor(sqrt(number)) with one BinarySearch.LowerBound
// over SqrtX's monotone SquareExceedsSequence - LC 69's own "does i^2 exceed x"
// witness, reused rather than copied, the same technique FourDivisors and
// ClosestDivisors use - then counts divisors off in sorted order: ascending up to the
// anchor, and past it by walking the anchor back down to 1 and reporting each partner
// `number` / divisor (also ascending, since `number` / divisor grows as divisor
// shrinks).
//
// Both strategies take LeetCode's own two integers, so neither needs a hoisted
// prepared-input overload - there is no input structure to build.
internal static class TheKthFactorOfNSolution
{
    // ceil(sqrt(int.MaxValue)): caps the binary-search anchor so squaring an index can
    // never overflow the search range.
    private const int SqrtAnchorCeiling = 46_341;

    // The textbook answer: trial-divide `number` by every candidate from 1 upward,
    // counting factors off until the `rank`-th appears. Deliberately written without
    // this repo's primitives - it is the arm the composed strategy below has to
    // justify itself against.
    public static int KthFactorByFullRangeScan(int number, int rank)
    {
        var remaining = rank;

        for (var divisor = 1; divisor <= number; divisor++)
        {
            if (number % divisor == 0 && --remaining == 0)
            {
                return divisor;
            }
        }

        return LeetCodeAnswer.None;
    }

    // Anchor at floor(sqrt(number)) in O(log n), then walk the two halves of the
    // divisor list in sorted order and stop the moment the `rank`-th factor is found -
    // never more than O(sqrt(number)) divisibility tests instead of a full scan.
    public static int KthFactorByBinarySearchAnchor(int number, int rank)
    {
        var anchor = SqrtAnchor(number);
        var remaining = rank;

        if (TryFindAtOrBelowAnchor(number, anchor, ref remaining, out var found))
        {
            return found;
        }

        return TryFindAboveAnchor(number, anchor, ref remaining, out found) ? found : LeetCodeAnswer.None;
    }

    // floor(sqrt(number)): the first index whose square exceeds `number`, less one.
    private static int SqrtAnchor(int number)
    {
        var sequence = new SquareExceedsSequence(number, Math.Min(number, SqrtAnchorCeiling) + 1);

        return BinarySearch.LowerBound<int, SquareExceedsSequence>(sequence, 1) - 1;
    }

    // The small half of the divisor list, already in ascending order.
    private static bool TryFindAtOrBelowAnchor(int number, int anchor, ref int remaining, out int found)
    {
        for (var divisor = 1; divisor <= anchor; divisor++)
        {
            if (number % divisor == 0 && --remaining == 0)
            {
                found = divisor;
                return true;
            }
        }

        found = LeetCodeAnswer.None;
        return false;
    }

    // The large half: walking the anchor back down to 1 and reporting `number` / divisor
    // visits the divisors above sqrt(number) in ascending order. A perfect square's root
    // is its own partner and was already counted above, so it is skipped here.
    private static bool TryFindAboveAnchor(int number, int anchor, ref int remaining, out int found)
    {
        for (var divisor = anchor; divisor >= 1; divisor--)
        {
            if (divisor * divisor == number || number % divisor != 0)
            {
                continue;
            }

            if (--remaining == 0)
            {
                found = number / divisor;
                return true;
            }
        }

        found = LeetCodeAnswer.None;
        return false;
    }
}
