using ActiveFlipsQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.LeetCode.MinimumNumberOfKConsecutiveBitFlips;

// LeetCode 995. Minimum Number of K Consecutive Bit Flips: the fewest flips of a
// window of length windowSize that turn a binary array into all ones, or -1 when no
// sequence of flips can.
//
// Both strategies run the same greedy - scanning left to right, a position that
// still reads 0 must be the left edge of a flip, because nothing further right can
// reach back to cover it - and differ only in how they learn what the current
// position reads. The baseline actually rewrites the windowSize-length window (O(n*k)); the
// composed strategy never touches the array, tracking which flips are still in
// range through this repo's own Queue<TElement> of flip start indices, whose Count
// parity is the cumulative flip state at the current position in O(1).
internal static class MinimumNumberOfKConsecutiveBitFlipsSolution
{
    private const int ParityModulus = 2;

    // The textbook answer: flip each window in place and check the result. Kept to
    // BCL arrays and index arithmetic - it is the arm the queue strategy below has
    // to justify itself against. Clones the input so the caller's array is left
    // alone, which the in-place rewrite would otherwise destroy.
    public static int MinKBitFlipsByInPlaceWindowFlip(int[] nums, int windowSize)
    {
        var bits = (int[])nums.Clone();
        var flipCount = FlipEveryUncoveredZero(bits, windowSize);

        return IsAllOnes(bits) ? flipCount : LeetCodeAnswer.None;
    }

    private static int FlipEveryUncoveredZero(int[] bits, int windowSize)
    {
        var flipCount = 0;

        for (var i = 0; i <= bits.Length - windowSize; i++)
        {
            if (bits[i] == 0)
            {
                FlipWindow(bits, i, windowSize);
                flipCount++;
            }
        }

        return flipCount;
    }

    private static void FlipWindow(int[] bits, int start, int windowSize)
    {
        for (var j = start; j < start + windowSize; j++)
        {
            bits[j] ^= 1;
        }
    }

    private static bool IsAllOnes(int[] bits)
    {
        foreach (var bit in bits)
        {
            if (bit == 0)
            {
                return false;
            }
        }

        return true;
    }

    // One left-to-right sweep over the untouched input. The queue holds the start
    // index of every flip still covering the current position; an entry expires
    // exactly windowSize positions after it started, so Count never exceeds windowSize
    // and its parity is what the current bit has been flipped to.
    public static int MinKBitFlipsByQueueTrackedParity(int[] nums, int windowSize)
    {
        var activeFlips = new ActiveFlipsQueue();
        var flipCount = 0;

        for (var i = 0; i < nums.Length; i++)
        {
            ExpireFlip(activeFlips, windowSize, i);

            var flipped = TryApplyFlip(activeFlips, nums, windowSize, i);
            if (flipped is null)
            {
                return LeetCodeAnswer.None;
            }

            flipCount += flipped.Value;
        }

        return flipCount;
    }

    private static void ExpireFlip(ActiveFlipsQueue activeFlips, int windowSize, int position)
    {
        if (activeFlips.TryPeek(out var earliestStart) && earliestStart + windowSize == position)
        {
            activeFlips.TryDequeue(out _);
        }
    }

    // Null means "this position reads 0 but no window of length windowSize starting
    // here fits", which is the unsolvable case; 0 and 1 are the flips spent at this
    // position.
    private static int? TryApplyFlip(ActiveFlipsQueue activeFlips, int[] nums, int windowSize, int position)
    {
        var effectiveBit = nums[position] ^ (activeFlips.Count % ParityModulus);

        if (effectiveBit != 0)
        {
            return 0;
        }

        if (position + windowSize > nums.Length)
        {
            return null;
        }

        activeFlips.Enqueue(position);
        return 1;
    }
}
