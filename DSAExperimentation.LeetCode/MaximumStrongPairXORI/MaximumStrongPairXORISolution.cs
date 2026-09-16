using DSAExperimentation.LeetCode.MaximumStrongPairXORII;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.MaximumStrongPairXORI;

// LeetCode 2932. Maximum Strong Pair XOR I: (x, y) is a strong pair when
// |x - y| <= min(x, y); return the maximum x XOR y over every strong pair.
// nums.Length <= 50 and every value <= 100 here, so the textbook O(n^2) pairwise
// scan is already fast enough on its own - but the BitTrie-bucket strategy LC
// 2935 actually needs to clear ITS bound is exactly as correct at this smaller
// one, so this class still offers both arms, and both are proven at LC 2932's
// own scale by MaximumStrongPairXORITests. The bucket strategy itself is
// MaximumStrongPairXORIISolution's - the two-lemma argument for it is stated at
// LC 2935's bound because that is the bound that needs it - so this class's
// bucket arms call through rather than restating the sweep at a second scale.
internal static class MaximumStrongPairXORISolution
{
    public static int MaximumStrongPairXorByBruteForce(int[] nums)
    {
        var best = 0;

        for (var i = 0; i < nums.Length; i++)
        {
            for (var j = i + 1; j < nums.Length; j++)
            {
                if (IsStrongPair(nums[i], nums[j]))
                {
                    best = Math.Max(best, nums[i] ^ nums[j]);
                }
            }
        }

        return best;
    }

    private static bool IsStrongPair(int firstValue, int secondValue)
    {
        var (small, large) = firstValue <= secondValue
            ? SmallestFirst(firstValue, secondValue)
            : LargestFirst(firstValue, secondValue);
        return large - small <= small;
    }

    // The pair with the smaller value first, or with the larger value first.
    private static (int Small, int Large) SmallestFirst(int firstValue, int secondValue) =>
        (firstValue, secondValue);

    private static (int Small, int Large) LargestFirst(int firstValue, int secondValue) =>
        (secondValue, firstValue);

    // LC 2935's own bound is what makes its class the one implementation of the
    // bucket strategy, so this arm calls it. Nothing narrows: both problems answer
    // an int over the same value range, and the buckets, the BitTrie composition
    // and the insert-only two-pointer sweep are all that class's.
    public static int MaximumStrongPairXorByBitTrieBuckets(int[] nums) =>
        MaximumStrongPairXORIISolution.MaximumStrongPairXorByBitTrieBuckets(nums);

    // Prepared-input overload: `sortedNums` must already be sorted ascending -
    // the same precondition MaximumStrongPairXORIISolution states for it.
    public static int MaximumStrongPairXorByBitTrieBuckets(ArrayIndexedSequence<int> sortedNums) =>
        MaximumStrongPairXORIISolution.MaximumStrongPairXorByBitTrieBuckets(sortedNums);
}
