using System.Numerics;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximizeNumberOfNiceDivisors;

// LeetCode 1808. Maximize Number of Nice Divisors: a "nice" divisor of
// n = p1^a1 * p2^a2 * ... must contain every one of n's prime factors at least
// once, so the count of nice divisors is exactly the product of the a_i (each
// exponent contributes a_i legal choices, 1..a_i) - the same "split n into parts
// maximizing their product" shape as Integer Break (LC 343), just for an n up to
// 1e9 instead of 58. This repo's own Memoizer (Algorithms/DynamicProgramming/
// Memoizer.cs) drives the recurrence maxProduct(remaining) = max(remaining,
// 2*maxProduct(remaining-2), 3*maxProduct(remaining-3)) - many different peel-2/
// peel-3 orders land on the same remaining budget, exactly the overlapping-
// subproblem shape memoization exists for (same idiom CountAllPossibleRoutesTests
// uses over a different state shape). BigInteger, not long, keeps every candidate's
// max comparison exact for however far the recursion runs - this composition
// proves correct, not that it matches LeetCode's own O(log n) modular-
// exponentiation scale (see the benchmark for the naive-vs-memoized complexity
// split, bounded well below 1e9 for the same reason).
public sealed partial class MaximizeNumberOfNiceDivisorsTests
{
    private const int Modulo = 1_000_000_007;

    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(4, 4)]
    [InlineData(5, 6)]
    [InlineData(8, 18)]
    public void MaxNiceDivisors_KnownExamples_ReturnsExpectedCount(int primeFactors, int expected)
        => Assert.Equal(expected, MaxNiceDivisors(primeFactors));

    private static int MaxNiceDivisors(int primeFactors)
        => (int)(MaxProduct(primeFactors) % Modulo);

    private static BigInteger MaxProduct(int remaining)
        => Memoizer.Memoize<int, BigInteger>(remaining, Recurrence);

    private static BigInteger Recurrence(int remaining, Func<int, BigInteger> maxProduct)
    {
        if (remaining == 0)
        {
            return BigInteger.One;
        }

        var best = (BigInteger)remaining;

        if (remaining >= 2)
        {
            best = BigInteger.Max(best, 2 * maxProduct(remaining - 2));
        }

        if (remaining >= 3)
        {
            best = BigInteger.Max(best, 3 * maxProduct(remaining - 3));
        }

        return best;
    }
}
