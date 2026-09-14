using DSAExperimentation.LeetCode.TheNumberOfGoodSubsets;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TheNumberOfGoodSubsets;

// Harness only. Both the unmemoized recursion and the Memoizer-backed one are
// TheNumberOfGoodSubsetsSolution's - this file pins them to LeetCode's two published
// examples plus the cases the examples never reach: a value with a repeated prime
// factor, duplicate values counted by index, 1s doubling the answer, three values that
// pairwise share a prime, and all ten primes below 30 at once.
public sealed class TheNumberOfGoodSubsetsTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            // LeetCode example 1: {2}, {3}, {2,3} and each of those with the 1 as well.
            // 4 = 2^2 is never usable.
            { [1, 2, 3, 4], 6 },

            // LeetCode example 2: {2}, {3}, {15}, {2,3}, {2,15} - {3,15} share the
            // prime 3, and 4 is again unusable.
            { [4, 2, 3, 15], 5 },

            // 1 alone is not a product of one or more primes, so it is not a good subset.
            { [1], 0 },

            // Every 1 doubles the answer, and on its own doubles nothing.
            { [1, 1, 1], 0 },

            // A repeated prime factor disqualifies the value outright.
            { [4], 0 },
            { [12, 9, 25, 8], 0 },

            // The single smallest good subset.
            { [2], 1 },

            // Subsets are distinguished by index, so two equal values give two subsets -
            // and {2,2} has product 4, which is not squarefree.
            { [2, 2], 2 },

            // {2}, {1,2} twice over: the recursion counts one, the 1s double it twice.
            { [1, 1, 2], 4 },

            // 30 = 2 * 3 * 5, three distinct primes in one value.
            { [30], 1 },

            // 6, 10 and 15 pairwise share a prime, so only the singletons survive.
            { [6, 10, 15], 3 },

            // All ten primes below 30: every non-empty subset of them is good.
            { [2, 3, 5, 7, 11, 13, 17, 19, 23, 29], 1023 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumberOfGoodSubsetsByBruteForceRecursion_LeetCodeExamples_ReturnsGoodSubsetCount(
        int[] nums, int expected) =>
        Assert.Equal(expected, TheNumberOfGoodSubsetsSolution.NumberOfGoodSubsetsByBruteForceRecursion(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumberOfGoodSubsetsByMemoizedRecursion_LeetCodeExamples_ReturnsGoodSubsetCount(
        int[] nums, int expected) =>
        Assert.Equal(expected, TheNumberOfGoodSubsetsSolution.NumberOfGoodSubsetsByMemoizedRecursion(nums));
}
