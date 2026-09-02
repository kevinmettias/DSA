using BenchmarkDotNet.Attributes;
using System.Numerics;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximize Number of Nice Divisors (LC 1808): the naive un-memoized recursion over
// the remaining budget - re-exploring the same `remaining` state on every peel-2/
// peel-3 order that reaches it, the same "no cache" shape FibonacciNumberBenchmarks'
// own baseline uses - vs. this repo's own Memoizer keyed on the int `remaining`
// state (MaximizeNumberOfNiceDivisorsTests precedent). PrimeFactors stays small
// enough that the naive side's ~1.33^n blowup still finishes in reasonable time
// while remaining clearly exponential next to the memoized side's O(n).
[MemoryDiagnoser]
public class MaximizeNumberOfNiceDivisorsBenchmarks
{
    private const int FactorOfTwo = 2;
    private const int FactorOfThree = 3;

    [Params(40, 50)]
    public int PrimeFactors;

    [Benchmark(Baseline = true)]
    public BigInteger NaiveRecursion() => MaxProductNaive(PrimeFactors);

    private static BigInteger MaxProductNaive(int remaining) => BestSplit(remaining, MaxProductNaive);

    [Benchmark]
    public BigInteger MemoizedTopDown() => Memoizer.Memoize<int, BigInteger>(PrimeFactors, Recurrence);

    private static BigInteger Recurrence(int remaining, Func<int, BigInteger> maxProduct) =>
        BestSplit(remaining, maxProduct);

    // Shared shape between the naive self-recursion and the memoized recurrence:
    // peel off either a factor of 2 or a factor of 3 and keep whichever split
    // yields the larger product.
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
