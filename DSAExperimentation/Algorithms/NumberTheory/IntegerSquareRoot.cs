using DSAExperimentation.Algorithms.Searching;

namespace DSAExperimentation.Algorithms.NumberTheory;

// floor(sqrt(value)) for a non-negative int, by binary search: the answer is one below the first
// index whose square exceeds the value. The search runs over a virtual monotone sequence - index i
// reads 1 when i^2 > value, else 0 - so nothing is materialized and BinarySearch.LowerBound does all
// the work, exactly as SqrtX's composed arm always did.
//
// The search is capped at ceil(sqrt(int.MaxValue)) = 46,341, so squaring an index can never leave
// long and the answer for int.MaxValue itself (46,340) is still in range.
internal static class IntegerSquareRoot
{
    private const int SquareRootCeiling = 46_341;

    public static int Floor(int value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);

        var sequence = new SquareExceedsSequence(value, Math.Min(value, SquareRootCeiling) + 1);

        return BinarySearch.LowerBound<int, SquareExceedsSequence>(sequence, SquareExceedsSequence.Exceeds) - 1;
    }
}
