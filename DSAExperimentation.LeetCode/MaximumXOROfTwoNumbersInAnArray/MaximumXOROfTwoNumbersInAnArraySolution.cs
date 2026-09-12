using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.MaximumXOROfTwoNumbersInAnArray;

// LeetCode 421. Maximum XOR of Two Numbers in an Array: for every value, find the
// other value in the array that maximizes their XOR.
internal static class MaximumXOROfTwoNumbersInAnArraySolution
{
    // The textbook answer: check every pair directly, O(n^2). Deliberately written
    // without this repo's primitives - it is the arm the composed solution below has
    // to justify itself against.
    public static int FindMaximumXorByPairwiseScan(int[] nums)
    {
        var best = 0;

        for (var i = 0; i < nums.Length; i++)
        {
            for (var j = i + 1; j < nums.Length; j++)
            {
                best = Math.Max(best, nums[i] ^ nums[j]);
            }
        }

        return best;
    }

    // This repo's own BitTrie (DataStructures/Graph/Engines/Dags/Trees/BitTrie.cs),
    // built specifically for this problem's family: insert every candidate's 32-bit
    // pattern once (O(32) each), then for each value greedily walk toward the
    // OPPOSITE bit at every level to find the best achievable XOR against it in
    // O(32) per query - O(n) total instead of O(n^2).
    public static int FindMaximumXorByBitTrie(int[] nums)
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
