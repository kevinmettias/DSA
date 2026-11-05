using DSAExperimentation.LeetCode.Solutions.TheNumberOfGoodSubsets;

namespace DSAExperimentation.LeetCode.Solutions.Tests.TheNumberOfGoodSubsets;

// Harness only. Both the unmemoized recursion and the Memoizer-backed one are
// TheNumberOfGoodSubsetsSolution's - this file pins them to cases LeetCode's examples
// never reach: single values, a value with a repeated prime factor, duplicate values
// counted by index, 1s doubling the answer, and three values that pairwise share a
// prime. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class TheNumberOfGoodSubsetsSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
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
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void NumberOfGoodSubsetsByBruteForceRecursion_Cases_ReturnsGoodSubsetCount(
        int[] nums, int expected) =>
        Assert.Equal(expected, TheNumberOfGoodSubsetsSolution.NumberOfGoodSubsetsByBruteForceRecursion(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void NumberOfGoodSubsetsByMemoizedRecursion_Cases_ReturnsGoodSubsetCount(
        int[] nums, int expected) =>
        Assert.Equal(expected, TheNumberOfGoodSubsetsSolution.NumberOfGoodSubsetsByMemoizedRecursion(nums));
}
