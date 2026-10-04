using DSAExperimentation.Algorithms.Searching;

namespace DSAExperimentation.LeetCode.DivideTwoIntegers;

// LeetCode 29. Divide Two Integers: truncate dividend/divisor toward zero without
// using multiplication, division or mod - except for the baseline, which is
// exactly the built-in division this puzzle forbids, and exists to be beaten.
//
// The composed strategy turns the search into "find the last quotient candidate
// whose product with the divisor still fits within the dividend" and hands that
// monotone rule, with the candidate range, to this repo's own
// MonotonePredicateSearch.LastTrue.
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

        var quotient = Quotient(absDividend, absDivisor);

        return negative ? -quotient : quotient;
    }

    // Binary search over quotient candidates in [0, absDividend]: the product fits
    // up to the true quotient and overshoots from the next candidate on, so the last
    // candidate that fits is the answer. The range is searched in long because its
    // top, absDividend, can be 2^31 and a quotient of exactly int.MaxValue
    // (int.MaxValue / 1) must stay inside it. The result narrows to int safely: the
    // only quotient magnitude past int.MaxValue, 2^31 from int.MinValue / +-1, is
    // answered before the search.
    private static int Quotient(long absDividend, long absDivisor) =>
        (int)MonotonePredicateSearch.LastTrue(0L, absDividend, new ProductFitsDividend(absDivisor, absDividend));

    // Bespoke to LC 29: "does q's product with the divisor still fit within the
    // dividend" is this problem's own monotone rule - true from 0 up to the
    // quotient, false past it. Both factors stay at or below 2^31, so the product
    // cannot overflow long.
    private readonly struct ProductFitsDividend(long divisor, long dividend) : IMonotonePredicate<long>
    {
        public bool Holds(long quotient) => divisor * quotient <= dividend;
    }
}
