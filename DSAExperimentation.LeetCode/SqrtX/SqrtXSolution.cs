using DSAExperimentation.Algorithms.NumberTheory;

namespace DSAExperimentation.LeetCode.SqrtX;

// LeetCode 69. Sqrt(x): the floor of the square root of a non-negative integer,
// without using any built-in exponent/root operator.
//
// The binary-search strategy treats "does i^2 exceed value" as a monotone
// predicate over a virtual sequence and finds the first index where it flips - promoted to the
// library as IntegerSquareRoot.Floor, which this arm now calls; the baseline is simply the BCL's
// own floating-point sqrt, truncated toward zero.
internal static class SqrtXSolution
{
    public static int RootByMathSqrt(int value) => (int)Math.Sqrt(value);

    public static int RootByBinarySearch(int value) => IntegerSquareRoot.Floor(value);
}
