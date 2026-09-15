using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.LeetCode.SqrtX;

namespace DSAExperimentation.LeetCode.ClosestDivisors;

// LeetCode 1362. Closest Divisors: of num + 1 and num + 2, return the divisor pair
// whose two members are closest together.
//
// Both strategies solve each candidate independently and keep the tighter pair; they
// differ only in how they find the largest divisor at or below sqrt(candidate), whose
// partner is necessarily the closest possible pair for that candidate. The baseline
// scans the whole range; the composed strategy anchors at floor(sqrt(candidate)) via
// BinarySearch.LowerBound over SqrtX's SquareExceedsSequence - LC 69's own monotone
// "does i^2 exceed x" witness, reused rather than copied - then walks down a few
// steps to the first exact divisor.
internal static class ClosestDivisorsSolution
{
    // LeetCode asks about num + 1 and num + 2, so the second candidate is +2.
    private const int SecondCandidateOffset = 2;

    // ceil(sqrt(int.MaxValue)): caps the binary-search anchor so squaring an index
    // can never overflow the search range.
    private const int SqrtAnchorCeiling = 46_341;

    // The textbook answer: for each candidate, walk every i from 1 upward and keep
    // the tightest (i, candidate / i) pair found - O(candidate) per candidate, plain
    // BCL arithmetic with no search structure at all.
    public static (int First, int Second) ClosestPairByDivisorScan(int num) =>
        TighterOf(DivisorScanPairFor(num + 1), DivisorScanPairFor(num + SecondCandidateOffset));

    private static (int First, int Second) DivisorScanPairFor(int candidate)
    {
        var best = (First: 1, Second: candidate);

        for (var i = 1; i <= candidate; i++)
        {
            if (candidate % i != 0 || i > candidate / i)
            {
                continue;
            }

            if (candidate / i - i < best.Second - best.First)
            {
                best = (i, candidate / i);
            }
        }

        return best;
    }

    // Anchor at floor(sqrt(candidate)) with one binary search, then step down to the
    // first exact divisor - the same technique SqrtXSolution.RootByBinarySearch uses
    // for LC 69, over the same virtual sequence.
    public static (int First, int Second) ClosestPairByBinarySearchAnchor(int num) =>
        TighterOf(AnchoredPairFor(num + 1), AnchoredPairFor(num + SecondCandidateOffset));

    private static (int First, int Second) AnchoredPairFor(int candidate)
    {
        var sequence = new SquareExceedsSequence(candidate, Math.Min(candidate, SqrtAnchorCeiling) + 1);
        var divisor = BinarySearch.LowerBound<int, SquareExceedsSequence>(sequence, 1) - 1;

        while (candidate % divisor != 0)
        {
            divisor--;
        }

        return (divisor, candidate / divisor);
    }

    private static (int First, int Second) TighterOf((int First, int Second) lower, (int First, int Second) upper) =>
        IsTighter(upper, lower) ? upper : lower;

    private static bool IsTighter((int First, int Second) candidate, (int First, int Second) reference) =>
        candidate.Second - candidate.First < reference.Second - reference.First;
}
