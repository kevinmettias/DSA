using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.GreatestCommonDivisorTraversal;

// LeetCode 2709. Greatest Common Divisor Traversal: the same DisjointSet-over-shared-
// prime-factor composition LargestComponentSizeByCommonFactorTests/GCDSortOfAnArrayTests
// already prove out - union index i with the first-seen index that shares any of
// nums[i]'s prime factors (a HashMap<factor,index> tracks the owner) - except this
// problem only needs to know whether every index ends up in ONE component, not the
// size of the largest one. gcd(nums[i], nums[j]) > 1 iff the two values share a prime
// factor, so "can traverse between every pair of indices" is exactly "is the whole
// array one connected component" under this union.
public sealed partial class GreatestCommonDivisorTraversalTests
{
    [Theory]
    [InlineData(new[] { 2, 3, 6 }, true)]
    [InlineData(new[] { 3, 9, 5 }, false)]
    [InlineData(new[] { 4, 3, 12, 8 }, true)]
    public void CanTraverseAllPairs_LeetCodeExamples_MatchesExpected(int[] nums, bool expected)
        => Assert.Equal(expected, CanTraverseAllPairs(nums));

    [Fact]
    public void CanTraverseAllPairs_SingleElement_TriviallyTrue()
        => Assert.True(CanTraverseAllPairs([7]));

    [Fact]
    public void CanTraverseAllPairs_ValueOfOneHasNoFactorsToShare_StaysIsolated()
        => Assert.False(CanTraverseAllPairs([1, 2, 4]));

    private static bool CanTraverseAllPairs(int[] nums)
    {
        var components = new DisjointSet(nums.Length);
        UnionSharedFactors(nums, components);

        for (var i = 1; i < nums.Length; i++)
        {
            if (!components.IsConnected(0, i))
            {
                return false;
            }
        }

        return true;
    }

    private static void UnionSharedFactors(int[] nums, DisjointSet components)
    {
        var firstIndexWithFactor = new HashMap<int, int>();

        for (var i = 0; i < nums.Length; i++)
        {
            foreach (var factor in PrimeFactors(nums[i]))
            {
                if (firstIndexWithFactor.TryGetValue(factor, out var owner))
                {
                    components.Union(i, owner);
                }
                else
                {
                    firstIndexWithFactor.Set(factor, i);
                }
            }
        }
    }

    private static IEnumerable<int> PrimeFactors(int value)
    {
        for (var factor = 2; factor * factor <= value; factor++)
        {
            if (value % factor != 0)
            {
                continue;
            }

            yield return factor;

            while (value % factor == 0)
            {
                value /= factor;
            }
        }

        if (value > 1)
        {
            yield return value;
        }
    }
}
