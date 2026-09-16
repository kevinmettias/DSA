using DSAExperimentation.LeetCode.ClosestPrimeNumbersInRange;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ClosestPrimeNumbersInRange;

// Harness only. Both strategies are ClosestPrimeNumbersInRangeSolution's - the
// per-candidate trial division that used to be the benchmark's unasserted baseline
// arm, and the DynamicArray<bool> sieve this file used to inline. The benchmark's
// two arms only ever returned the smallest gap; both now return LeetCode's actual
// answer, the pair itself, so the tie rule is under test too.
public sealed class ClosestPrimeNumbersInRangeTests
{
    public static TheoryData<int, int, int[]> Examples =>
        new()
        {
            // LC examples 1-2.
            { 10, 19, [11, 13] },
            { 4, 6, [-1, -1] },

            // The smallest range that holds a pair at all, and the only pair of
            // consecutive integers that are both prime.
            { 2, 3, [2, 3] },

            // left below 2: the sieve and the scan both have to skip 0 and 1.
            { 1, 4, [2, 3] },

            // Tie: 3-5 and 5-7 are both gaps of 2, and the smallest first element
            // wins, so the earlier pair is kept.
            { 3, 7, [3, 5] },

            // The closest pair is the last one in the range, not the first.
            { 19, 31, [29, 31] },

            // A twin pair well inside a wide range.
            { 100, 200, [101, 103] },

            // Exactly one prime in range, so there is no pair to report.
            { 13, 13, [-1, -1] },

            // No primes at all.
            { 1, 1, [-1, -1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ClosestPrimesByTrialDivision_LeetCodeExamples_ReturnsClosestPair(
        int left, int right, int[] expected)
    {
        var actual = ClosestPrimeNumbersInRangeSolution.ClosestPrimesByTrialDivision(left, right);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ClosestPrimesBySieve_LeetCodeExamples_ReturnsClosestPair(
        int left, int right, int[] expected)
    {
        var actual = ClosestPrimeNumbersInRangeSolution.ClosestPrimesBySieve(left, right);

        Assert.Equal(expected, actual);
    }
}
