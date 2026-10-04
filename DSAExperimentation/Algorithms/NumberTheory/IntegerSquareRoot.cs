using DSAExperimentation.Algorithms.Searching;

namespace DSAExperimentation.Algorithms.NumberTheory;

// floor(sqrt(value)) for a non-negative int, by binary search on the answer: the floor is one below the
// first candidate whose square exceeds the value. MonotonePredicateSearch.FirstTrue walks the
// candidates [0, min(value, cap)] against SquareExceeds, so nothing is materialized and the search
// does all the work, exactly as SqrtX's composed arm always did.
//
// The search is capped at ceil(sqrt(int.MaxValue)) = 46,341, so squaring a candidate can never leave
// long and the answer for int.MaxValue itself (46,340) is still in range. When no candidate in range
// exceeds - value 0 or 1 - FirstTrue answers one past the range, and the floor is the range's top.
internal static class IntegerSquareRoot
{
    private const int SquareRootCeiling = 46_341;

    public static int Floor(int value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);

        var lastCandidate = Math.Min(value, SquareRootCeiling);

        return MonotonePredicateSearch.FirstTrue(0, lastCandidate, new SquareExceeds(value)) - 1;
    }
}
