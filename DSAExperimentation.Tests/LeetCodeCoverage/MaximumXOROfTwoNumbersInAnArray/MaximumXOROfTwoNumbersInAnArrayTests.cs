using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumXOROfTwoNumbersInAnArray;

// LeetCode 421. Maximum XOR of Two Numbers in an Array: this repo's own BitTrie
// (DataStructures/Graph/Engines/Dags/Trees/BitTrie.cs) was built specifically for
// this problem's family - insert every candidate's 32-bit pattern once, then for
// each value greedily walk toward the OPPOSITE bit at every level to find the best
// achievable XOR against it in O(32) per query instead of the O(n^2) pairwise scan.
public sealed partial class MaximumXOROfTwoNumbersInAnArrayTests
{
    [Fact]
    public void FindMaximumXor_ClassicExample_ReturnsBestPairXor()
    {
        int[] nums = [3, 10, 5, 25, 2, 8];

        Assert.Equal(28, FindMaximumXor(nums));
    }

    [Fact]
    public void FindMaximumXor_TwoElements_ReturnsTheirXor()
    {
        int[] nums = [14, 70];

        Assert.Equal(14 ^ 70, FindMaximumXor(nums));
    }

    [Fact]
    public void FindMaximumXor_SingleElement_ReturnsZero()
    {
        int[] nums = [9];

        Assert.Equal(0, FindMaximumXor(nums));
    }

    private static int FindMaximumXor(int[] nums)
    {
        var trie = new BitTrie();

        foreach (var num in nums)
        {
            trie.Insert(num);
        }

        var best = 0;

        foreach (var num in nums)
        {
            if (trie.TryMaxXor(num, out var candidate))
            {
                best = Math.Max(best, candidate);
            }
        }

        return best;
    }
}
