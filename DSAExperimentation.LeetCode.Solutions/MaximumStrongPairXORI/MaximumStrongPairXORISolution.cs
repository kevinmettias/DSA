using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;
using DSAExperimentation.LeetCode.Solutions.MaximumStrongPairXORII;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.Solutions.MaximumStrongPairXORI;

// LeetCode 2932. Maximum Strong Pair XOR I: (x, y) is a strong pair when
// |x - y| <= min(x, y); return the maximum x XOR y over every strong pair.
// nums.Length <= 50 and every value <= 100 here, so the textbook O(n^2) pairwise
// scan is already fast enough on its own - but the BitTrie-bucket strategy LC
// 2935 actually needs to clear ITS bound is exactly as correct at this smaller
// one, so this class still offers both arms, and both are proven at LC 2932's
// own scale by CatalogCaseTests, over LC 2932's catalog cases. Both arms live
// with LC 2935 (ARCHITECTURE 17.3) - the two-lemma argument
// for the buckets is stated at LC 2935's bound because that is the bound that
// needs it - so this class calls through rather than restating either at a second
// scale.
[LeetCodeQuestion("maximum-strong-pair-xor-i", LeetCodeAnswerOrder.AsWritten)]
internal static class MaximumStrongPairXORISolution
{
    // The textbook O(n^2) scan of every unordered pair.
    public static int MaximumStrongPairXorByBruteForce(int[] nums) =>
        MaximumStrongPairXORIISolution.MaximumStrongPairXorByBruteForce(nums);

    // LC 2935's own bound is what makes its StrongPairBuckets the one
    // implementation of the bucket strategy, so this arm calls it. Nothing narrows:
    // both problems answer an int over the same value range, and the buckets and
    // the insert-only two-pointer sweep are all that class's. What LC 2932 chooses
    // is the set: at 50 values or fewer every bucket's set holds a handful, and
    // TwoFieldBitTrie, the hand-specialized set, inlines whole into the sweep, where
    // the one trie's layered walk runs out of the JIT's inlining budget
    // (ARCHITECTURE 13.9).
    public static int MaximumStrongPairXorByBitTrieBuckets(int[] nums) =>
        StrongPairBuckets<TwoFieldBitTrie>.MaxXor(nums);

    // Prepared-input overload: `sortedNums` must already be sorted ascending -
    // the same precondition MaximumStrongPairXORIISolution states for it.
    public static int MaximumStrongPairXorByBitTrieBuckets(ArrayIndexedSequence<int> sortedNums) =>
        StrongPairBuckets<TwoFieldBitTrie>.MaxXor(sortedNums);
}
