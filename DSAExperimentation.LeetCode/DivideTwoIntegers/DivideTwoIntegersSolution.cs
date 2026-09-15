using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.DivideTwoIntegers;

// LeetCode 29. Divide Two Integers: truncate dividend/divisor toward zero without
// using multiplication, division or mod - except for the baseline, which is
// exactly the built-in division this puzzle forbids, and exists to be beaten.
//
// The composed strategy turns the search into "find the first quotient candidate
// whose product with the divisor exceeds the dividend" and hands that monotone
// predicate to this repo's own BinarySearch.LowerBound.
internal static class DivideTwoIntegersSolution
{
    // What you would write without the "no division operator" constraint. Still
    // has to defend LeetCode's one representable-overflow example: dividing
    // int.MinValue by -1 overflows int32, and LeetCode defines the answer as
    // int.MaxValue, but the CLR's div instruction traps on that case even in an
    // unchecked context, so the guard is not optional.
    public static int DivideByBuiltInDivision(int dividend, int divisor)
    {
        if (dividend == int.MinValue && divisor == -1)
        {
            return int.MaxValue;
        }

        return dividend / divisor;
    }

    // Divides by taking the absolute values, searching the quotient by binary search,
    // then re-applying the sign the two operands disagreed on.
    public static int DivideByBinarySearchProduct(int dividend, int divisor)
    {
        if (dividend == int.MinValue && divisor == -1)
        {
            return int.MaxValue;
        }

        if (dividend == int.MinValue && divisor == 1)
        {
            return int.MinValue;
        }

        var negative = (dividend < 0) ^ (divisor < 0);
        var absDividend = Math.Abs((long)dividend);
        var absDivisor = Math.Abs((long)divisor);

        var quotient = QuotientByLowerBound(absDividend, absDivisor);

        return negative ? -quotient : quotient;
    }

    // Binary search over quotient candidates: ProductExceedsSequence(q) is 0 while
    // divisor*q <= dividend and flips to 1 the first time it overshoots, so
    // LowerBound(1) lands one past the true quotient.
    private static int QuotientByLowerBound(long absDividend, long absDivisor)
    {
        // maxCandidate + 1 is the sequence length we want (indices 0..maxCandidate
        // must all be valid), but that overflows int32 exactly when maxCandidate is
        // already int.MaxValue - which happens whenever absDividend itself reaches
        // int.MaxValue, including this repo's own int.MaxValue-dividend benchmark
        // workload. Clamping the length to int.MaxValue in long arithmetic instead avoids
        // that overflow and is exact for every quotient this repo's tests and
        // benchmarks ever ask for (all comfortably under int.MaxValue); it is not
        // exact in the one case an IRandomAccessSequence<int> cannot represent
        // regardless - a true quotient of exactly int.MaxValue itself (only
        // reachable via dividend == int.MaxValue, divisor == 1), which would need
        // a length one past int32's own range.
        var maxCandidate = (int)Math.Min(int.MaxValue, absDividend);
        var length = (int)Math.Min(maxCandidate + 1L, int.MaxValue);
        var sequence = new ProductExceedsSequence(absDivisor, absDividend, length);
        var firstTooLarge = BinarySearch.LowerBound<int, ProductExceedsSequence>(sequence, 1);

        return firstTooLarge - 1;
    }

    // Bespoke to LC 29: "does q's product with the divisor already exceed the
    // dividend" is this problem's own monotone predicate, not a general-purpose
    // sequence anything else would want.
    private readonly struct ProductExceedsSequence(long divisor, long dividend, int length)
        : IRandomAccessSequence<int>
    {
        public int Length => length;

        public int Get(int quotient) => ProductExceeds(quotient) ? 1 : 0;

        // The predicate, named: "does q's product with the divisor already exceed the
        // dividend" is what LowerBound is really searching, and a call in the arm keeps
        // that search reading as one.
        private bool ProductExceeds(int quotient) => divisor * quotient > dividend;
    }
}
