using DSAExperimentation.DataStructures.CountedBitTrie;

namespace DSAExperimentation.LeetCode.CountPairsWithXorInARange;

// LeetCode 1803. Count Pairs With XOR in a Range: how many index pairs i < j have
// low <= nums[i] ^ nums[j] <= high.
//
// Both strategies answer the same counting question and differ only in how they
// find the pairs: rescanning every earlier value, or this repo's own
// CountedBitTrie, which counts a whole same-bit subtree in one step.
internal static class CountPairsWithXorInARangeSolution
{
    // The textbook answer: XOR every pair and test the range. Deliberately written
    // over nothing but the input array - it is the O(n^2) arm the trie below has
    // to justify itself against.
    public static int CountPairsByPairwiseScan(int[] nums, int low, int high)
    {
        var pairs = 0;

        for (var i = 0; i < nums.Length; i++)
        {
            for (var j = i + 1; j < nums.Length; j++)
            {
                var xor = nums[i] ^ nums[j];

                if (xor >= low && xor <= high)
                {
                    pairs++;
                }
            }
        }

        return pairs;
    }

    // This repo's own CountedBitTrie (DataStructures/CountedBitTrie/CountedBitTrie.cs), whose
    // every node counts the values passing through it: CountXorBelow descends once per query
    // toward the bit that keeps the running XOR equal to the limit and, at every level where the
    // limit's bit is 1, adds the whole same-bit subtree's count in one read - the classic trie
    // range-count technique, O(32) per insert and query instead of an O(n) pairwise rescan.
    // low <= xor <= high pairs = CountXorBelow(high + 1) - CountXorBelow(low), summed while
    // inserting one value at a time so every i < j pair is counted exactly once.
    public static int CountPairsByBitTrieRangeCount(int[] nums, int low, int high)
    {
        var earlier = new CountedBitTrie();
        var pairs = 0;

        foreach (var value in nums)
        {
            var atMostHigh = earlier.CountXorBelow(value, high + 1);
            var belowLow = earlier.CountXorBelow(value, low);
            pairs += atMostHigh - belowLow;
            earlier.Insert(value);
        }

        return pairs;
    }
}
