using DSAExperimentation.Algorithms.Searching;

namespace DSAExperimentation.LeetCode.SqrtX;

// LeetCode 69. Sqrt(x): the floor of the square root of a non-negative integer,
// without using any built-in exponent/root operator.
//
// The binary-search strategy treats "does i^2 exceed x" as a monotone predicate
// over a virtual sequence and finds the first index where it flips; the baseline
// is simply the BCL's own floating-point sqrt, truncated toward zero.
internal static class SqrtXSolution
{
    // The largest x that fits in a signed 32-bit int has a floor sqrt of 46340
    // (46341^2 already overflows int.MaxValue), so the search never needs more than
    // 46341 candidates no matter how large x is.
    private const int MaxSqrtCandidate = 46341;

    public static int RootByMathSqrt(int x) => (int)Math.Sqrt(x);

    public static int RootByBinarySearch(int x)
    {
        var sequence = new SquareExceedsSequence(x, Math.Min(x, MaxSqrtCandidate) + 1);
        return BinarySearch.LowerBound<int, SquareExceedsSequence>(sequence, 1) - 1;
    }
}
