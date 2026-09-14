using System.Numerics;
using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.MaximizeNumberOfNiceDivisors;

// LeetCode 1808. Maximize Number of Nice Divisors: a "nice" divisor of
// n = p1^a1 * p2^a2 * ... must contain every one of n's prime factors at least
// once, so the count of nice divisors is exactly the product of the a_i (each
// exponent contributes a_i legal choices, 1..a_i). Choosing the exponents is
// therefore the "split the budget into parts maximizing their product" shape of
// Integer Break (LC 343), and LeetCode reports the answer modulo 1e9+7.
//
// Both strategies evaluate the same recurrence -
// maxProduct(remaining) = max(remaining, 2 * maxProduct(remaining - 2),
// 3 * maxProduct(remaining - 3)) - and differ only in whether a state reached by
// two different peel-2/peel-3 orders is recomputed or remembered. BigInteger, not
// long, keeps every candidate's max comparison exact however far the recursion
// runs; the modulo is applied once at the end, where LeetCode's reporting
// convention belongs.
internal static class MaximizeNumberOfNiceDivisorsSolution
{
    private const int FactorOfTwo = 2;
    private const int FactorOfThree = 3;

    // The textbook answer: plain self-recursion with no cache, re-exploring the
    // same `remaining` state on every peel order that reaches it. Deliberately
    // written over nothing but BigInteger and the call stack - it is the ~1.33^n
    // arm the memoized composition below has to justify itself against.
    public static int MaxNiceDivisorsByNaiveRecursion(int primeFactors)
        => ReportedAnswer(MaxProductNaive(primeFactors));

    private static BigInteger MaxProductNaive(int remaining) => BestSplit(remaining, MaxProductNaive);

    // This repo's own Memoizer (Algorithms/DynamicProgramming/Memoizer.cs) keyed on
    // the int `remaining` state: many different peel-2/peel-3 orders land on the
    // same budget, which is exactly the overlapping-subproblem shape memoization
    // exists for, collapsing the naive arm's exponential tree to O(n) states - the
    // same idiom CountAllPossibleRoutes uses over a wider state shape.
    public static int MaxNiceDivisorsByMemoizedRecurrence(int primeFactors)
        => ReportedAnswer(Memoizer.Memoize<int, BigInteger>(primeFactors, Recurrence));

    private static BigInteger Recurrence(int remaining, Func<int, BigInteger> maxProduct)
        => BestSplit(remaining, maxProduct);

    private static int ReportedAnswer(BigInteger maxProduct) => (int)(maxProduct % ModularArithmetic.Modulo);

    // Shared shape between the naive self-recursion and the memoized recurrence:
    // peel off either a factor of 2 or a factor of 3 and keep whichever split
    // yields the larger product, against leaving the budget whole.
    private static BigInteger BestSplit(int remaining, Func<int, BigInteger> maxProduct)
    {
        if (remaining == 0)
        {
            return BigInteger.One;
        }

        var best = (BigInteger)remaining;

        if (remaining >= FactorOfTwo)
        {
            best = BigInteger.Max(best, FactorOfTwo * maxProduct(remaining - FactorOfTwo));
        }

        if (remaining >= FactorOfThree)
        {
            best = BigInteger.Max(best, FactorOfThree * maxProduct(remaining - FactorOfThree));
        }

        return best;
    }
}
