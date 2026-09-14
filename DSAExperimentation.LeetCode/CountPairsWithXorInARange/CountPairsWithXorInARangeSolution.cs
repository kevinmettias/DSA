using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.CountPairsWithXorInARange;

// LeetCode 1803. Count Pairs With XOR in a Range: how many index pairs i < j have
// low <= nums[i] ^ nums[j] <= high.
//
// Both strategies answer the same counting question and differ only in how they
// find the pairs: rescanning every earlier value, or annotating this repo's own
// BitTrie so a whole same-bit subtree can be counted in one step.
internal static class CountPairsWithXorInARangeSolution
{
    // BitTrie stores int's full two's-complement bit pattern, most significant
    // bit first, so every walk over it runs from bit 31 down to bit 0.
    private const int TopBitIndex = 31;

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

    // This repo's own BitTrie (DataStructures/Graph/Engines/Dags/Trees/BitTrie.cs,
    // built for LC 421's family) plus HashMap<TKey,TValue> keyed by BitTrieNode
    // reference identity: Insert below walks the SAME 32-bit, MSB-first bit path
    // BitTrie.Insert itself already walks, incrementing a per-node "how many
    // inserted values pass through here" counter alongside it. CountLessThan then
    // descends once per query toward the bit that keeps the running XOR below the
    // limit and, at every level where the limit's bit is 1, adds the WHOLE same-bit
    // subtree's count in O(1) via that counter - the classic trie range-count
    // technique, O(32) per insert/query instead of an O(n) pairwise rescan.
    // low <= xor <= high pairs = CountLessThan(high + 1) - CountLessThan(low),
    // summed while inserting one value at a time so every i < j pair is counted
    // exactly once.
    public static int CountPairsByBitTrieRangeCount(int[] nums, int low, int high)
    {
        var trie = new BitTrie();
        var subtreeCount = new HashMap<BitTrieNode, int>();
        var pairs = 0;

        foreach (var value in nums)
        {
            pairs += CountLessThan(trie, subtreeCount, value, high + 1) - CountLessThan(trie, subtreeCount, value, low);
            Insert(trie, subtreeCount, value);
        }

        return pairs;
    }

    private static void Insert(BitTrie trie, HashMap<BitTrieNode, int> subtreeCount, int value)
    {
        trie.Insert(value);

        var current = trie.Root;
        var bits = unchecked((uint)value);

        for (var i = TopBitIndex; i >= 0; i--)
        {
            var bit = (bits >> i) & 1u;
            current = bit == 0 ? current.Zero! : current.One!;
            subtreeCount.TryGetValue(current, out var existing);
            subtreeCount.Set(current, existing + 1);
        }
    }

    // Counts previously-inserted values y with (value ^ y) < limit.
    private static int CountLessThan(BitTrie trie, HashMap<BitTrieNode, int> subtreeCount, int value, int limit)
    {
        if (limit <= 0 || trie.Count == 0)
        {
            return 0;
        }

        var context = new BitScanContext(subtreeCount, unchecked((uint)value), unchecked((uint)limit));
        BitTrieNode? current = trie.Root;
        var count = 0;

        for (var i = TopBitIndex; i >= 0 && current is not null; i--)
        {
            var (next, contributed) = AdvanceLevel(context, current, i);
            current = next;
            count += contributed;
        }

        return count;
    }

    private readonly record struct BitScanContext(HashMap<BitTrieNode, int> SubtreeCount, uint ValueBits, uint LimitBits);

    private static (BitTrieNode? Next, int Contributed) AdvanceLevel(BitScanContext context, BitTrieNode current, int bitIndex)
    {
        var valueBit = (context.ValueBits >> bitIndex) & 1u;
        var limitBit = (context.LimitBits >> bitIndex) & 1u;
        var sameChild = valueBit == 0 ? current.Zero : current.One;
        var oppositeChild = valueBit == 0 ? current.One : current.Zero;

        if (limitBit != 1)
        {
            return (sameChild, 0);
        }

        var contributed = 0;

        if (sameChild is not null && context.SubtreeCount.TryGetValue(sameChild, out var sameCount))
        {
            contributed = sameCount;
        }

        return (oppositeChild, contributed);
    }
}
