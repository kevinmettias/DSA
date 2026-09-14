using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.LeetCode.SqrtX;

namespace DSAExperimentation.LeetCode.ThreeDivisors;

// LeetCode 1952. Three Divisors: report whether num has exactly three divisors -
// which happens exactly when num is the square of a prime, since its divisors are
// then 1, p and p^2.
//
// Both strategies answer that same question and differ only in where the divisor
// walk starts. The baseline trial-divides every candidate from 1 to num. The
// composed strategy anchors at floor(sqrt(num)) with one BinarySearch.LowerBound
// over SqrtX's monotone SquareExceedsSequence - LC 69's own "does i^2 exceed x"
// witness, reused rather than copied, the same technique FourDivisors (LC 1390),
// ClosestDivisors (LC 1362) and TheKthFactorOfN (LC 1492) use - then walks down
// collecting both members of each divisor pair, bailing out the moment a fourth
// divisor appears, so any number that is not a prime square stops paying almost
// immediately instead of scanning all the way to 1.
//
// Both strategies take LeetCode's own single integer, so neither needs a hoisted
// prepared-input overload - there is no input structure to build.
internal static class ThreeDivisorsSolution
{
    private const int TargetDivisorCount = 3;

    // A divisor below sqrt(num) and its partner above it are two distinct divisors.
    private const int DistinctDivisorPairCount = 2;

    // ceil(sqrt(int.MaxValue)): caps the binary-search anchor so squaring an index can
    // never overflow the search range.
    private const int SqrtAnchorCeiling = 46_341;

    // The textbook answer: trial-divide num by every candidate from 1 upward, plain
    // BCL arithmetic with no search structure at all, giving up as soon as a fourth
    // divisor appears. This is the arm the composed strategy has to justify itself
    // against.
    public static bool IsThreeByFullRangeScan(int num)
    {
        var count = 0;

        for (var divisor = 1; divisor <= num; divisor++)
        {
            if (num % divisor != 0)
            {
                continue;
            }

            count++;

            if (count > TargetDivisorCount)
            {
                return false;
            }
        }

        return count == TargetDivisorCount;
    }

    // Anchor at floor(sqrt(num)) in O(log num), then walk down collecting divisor
    // pairs - the square root itself is counted once, every smaller divisor twice
    // with its partner.
    public static bool IsThreeByBinarySearchAnchor(int num)
    {
        var sequence = new SquareExceedsSequence(num, Math.Min(num, SqrtAnchorCeiling) + 1);
        var anchor = BinarySearch.LowerBound<int, SquareExceedsSequence>(sequence, 1) - 1;

        var count = 0;

        for (var divisor = anchor; divisor >= 1; divisor--)
        {
            count = Accumulate(num, divisor, count);

            if (count > TargetDivisorCount)
            {
                return false;
            }
        }

        return count == TargetDivisorCount;
    }

    // Adds the divisor pair (divisor, num / divisor) to the running count, or one
    // divisor when num is a perfect square and the two members coincide.
    private static int Accumulate(int num, int divisor, int count)
    {
        if (num % divisor != 0)
        {
            return count;
        }

        var paired = num / divisor;

        return count + (divisor == paired ? 1 : DistinctDivisorPairCount);
    }
}
