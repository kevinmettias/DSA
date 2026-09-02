using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ClosestPrimeNumbersInRange;

// LeetCode 2523. Closest Prime Numbers in Range: the same Sieve of Eratosthenes
// CountPrimesTests already builds over this repo's own DynamicArray<bool> composite
// tracker, run once up to `right` and then scanned once for the smallest gap
// between consecutive primes inside [left, right] (first minimal gap wins, so ties
// keep the smallest/leftmost pair).
public sealed class ClosestPrimeNumbersInRangeTests
{
    public static TheoryData<int, int, int[]> Examples =>
        new()
        {
            { 10, 19, [11, 13] },
            { 4, 6, [-1, -1] },
            { 2, 3, [2, 3] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ClosestPrimes_LeetCodeExamples_ReturnsClosestPair(int left, int right, int[] expected)
        => Assert.Equal(expected, ClosestPrimes(left, right));

    private static int[] ClosestPrimes(int left, int right)
    {
        var isComposite = SieveComposites(right);
        var previousPrime = -1;
        var bestLow = -1;
        var bestHigh = -1;
        var bestGap = int.MaxValue;

        for (var candidate = Math.Max(left, 2); candidate <= right; candidate++)
        {
            if (isComposite.Get(candidate))
            {
                continue;
            }

            if (previousPrime != -1 && candidate - previousPrime < bestGap)
            {
                bestGap = candidate - previousPrime;
                bestLow = previousPrime;
                bestHigh = candidate;
            }

            previousPrime = candidate;
        }

        return [bestLow, bestHigh];
    }

    private static DynamicArray<bool> SieveComposites(int right)
    {
        var isComposite = new DynamicArray<bool>();
        for (var i = 0; i <= right; i++)
        {
            isComposite.Add(i < 2);
        }

        for (var i = 2; (long)i * i <= right; i++)
        {
            if (isComposite.Get(i))
            {
                continue;
            }

            for (var multiple = i * i; multiple <= right; multiple += i)
            {
                isComposite.Set(multiple, true);
            }
        }

        return isComposite;
    }
}
