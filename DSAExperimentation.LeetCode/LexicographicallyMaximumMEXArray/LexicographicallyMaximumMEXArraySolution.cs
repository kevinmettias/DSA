namespace DSAExperimentation.LeetCode.LexicographicallyMaximumMEXArray;

// LeetCode 3948. Lexicographically Maximum MEX Array: repeatedly cut a prefix
// off the front of nums, append the MEX of that prefix to result, until nums is
// empty; make result lexicographically largest.
//
// MEX is monotonically non-decreasing as a prefix grows (a longer prefix's
// value set only ever gains elements), so the largest MEX any prefix of the
// CURRENT remaining suffix can reach is exactly the MEX of the whole suffix -
// and once a growing prefix reaches that value it can never exceed it before
// running out of suffix. That makes the first element of an optimal move fixed:
// it has to be the whole remaining suffix's own MEX. Among every prefix length
// that reaches it, the shortest one is the right cut, because taking more than
// necessary only discards elements the next cut could otherwise still use - and
// since lexicographic comparison prefers a longer array once a shared prefix
// ties, leaving more for later can only help. Both arms below implement exactly
// that greedy cut; they differ only in how cheaply MEX gets recomputed.
internal static class LexicographicallyMaximumMEXArraySolution
{
    // Textbook baseline: at each cut point, rescan the entire remaining suffix
    // with a fresh HashSet to find its MEX (the target), then grow a window one
    // element at a time - each step rescanning that window's own MEX from
    // scratch too - until the target is reached. Quadratic (or worse) in the
    // suffix length; the arm the frequency/pointer strategy below has to beat.
    public static int[] MexArrayByBruteForce(int[] nums)
    {
        var result = new List<int>();
        var start = 0;

        while (start < nums.Length)
        {
            var target = MexOf(nums, start, nums.Length - 1);
            var cutLength = ShortestPrefixReaching(nums, start, target);

            result.Add(target);
            start += cutLength;
        }

        return [.. result];
    }

    private static int ShortestPrefixReaching(int[] nums, int start, int target)
    {
        for (var length = 1; ; length++)
        {
            if (MexOf(nums, start, start + length - 1) == target)
            {
                return length;
            }
        }
    }

    // Composed: two frequency-array/pointer sweeps, each O(n) amortized.
    // SuffixMex precomputes, for every start index, the MEX of nums[start..] in
    // one left-to-right pass - removing one element from a suffix can only drop
    // its MEX, and only ever drops it exactly to that element's own value (see
    // BuildSuffixMex's own comment). The main pass then grows each window with
    // a MEX pointer that only ever advances, so the whole array is visited
    // O(1) amortized times across every window combined.
    public static int[] MexArrayByFrequencyPointer(int[] nums)
    {
        var n = nums.Length;
        var suffixMex = BuildSuffixMex(nums);
        var result = new List<int>();
        var windowFrequency = new int[n + 1];
        var start = 0;

        while (start < n)
        {
            var target = suffixMex[start];
            var cutLength = GrowWindowToTarget(nums, start, target, windowFrequency);

            result.Add(target);
            start += cutLength;
        }

        return [.. result];
    }

    // suffixMex[i] = MEX(nums[i..n-1]). Starts from the whole array's MEX (one
    // full scan) and, moving left to right, removes nums[i] from the running
    // frequency count for each step to i + 1. If the removed value was >= the
    // current MEX, every value below the MEX is untouched and the MEX cannot
    // change. If it was < the current MEX and its count just hit zero, every
    // value below it is still present (the old MEX invariant guaranteed that),
    // so the new MEX is exactly the removed value - no rescan needed either way.
    private static int[] BuildSuffixMex(int[] nums)
    {
        var n = nums.Length;
        var frequency = CountValues(nums, n);

        var mexPointer = AdvanceMex(frequency, 0);

        var suffixMex = new int[n + 1];
        suffixMex[0] = mexPointer;

        for (var i = 0; i < n; i++)
        {
            mexPointer = RemoveValue(nums[i], frequency, mexPointer);

            suffixMex[i + 1] = mexPointer;
        }

        return suffixMex;
    }

    // How many times each value in 1..n occurs. A value above n can never be the
    // MEX of an n-element array, so it is not tracked at all.
    private static int[] CountValues(int[] nums, int n)
    {
        var frequency = new int[n + 1];

        foreach (var value in nums)
        {
            if (value <= n)
            {
                frequency[value]++;
            }
        }

        return frequency;
    }

    // The smallest value at or above `from` that is not present - the MEX, once
    // every value below it has been counted in.
    private static int AdvanceMex(int[] frequency, int from)
    {
        var n = frequency.Length - 1;
        var mexPointer = from;

        while (mexPointer <= n && frequency[mexPointer] > 0)
        {
            mexPointer++;
        }

        return mexPointer;
    }

    // Takes one occurrence of `value` back out of the counts and reports the MEX
    // that leaves: a removal at or above the current MEX cannot touch it, and one
    // below can only lower it to exactly `value` itself.
    private static int RemoveValue(int value, int[] frequency, int mexPointer)
    {
        var n = frequency.Length - 1;

        if (value > n)
        {
            return mexPointer;
        }

        frequency[value]--;

        if (frequency[value] == 0 && value < mexPointer)
        {
            mexPointer = value;
        }

        return mexPointer;
    }

    // Consumes at least one element (a suffix whose own MEX is 0 still needs a
    // 1-element cut), then keeps adding elements and advancing the window's own
    // MEX pointer until it reaches target - guaranteed to happen by the time the
    // whole remaining suffix is consumed, since target IS that suffix's MEX.
    private static int GrowWindowToTarget(int[] nums, int start, int target, int[] windowFrequency)
    {
        var windowMexPointer = 0;
        var index = start;

        do
        {
            (index, windowMexPointer) = AbsorbElement(nums, index, windowFrequency, windowMexPointer);
        }
        while (windowMexPointer < target);

        ClearWindow(nums, start, index, windowFrequency);

        return index - start;
    }

    // Takes nums[index] into the window and reports the index just past it along
    // with the window's own MEX pointer once that element has been counted.
    private static (int Index, int WindowMexPointer) AbsorbElement(
        int[] nums, int index, int[] windowFrequency, int windowMexPointer)
    {
        var n = nums.Length;
        var value = nums[index];

        if (value <= n)
        {
            windowFrequency[value]++;
        }

        var nextIndex = index + 1;

        while (windowMexPointer <= n && windowFrequency[windowMexPointer] > 0)
        {
            windowMexPointer++;
        }

        return (nextIndex, windowMexPointer);
    }

    // The window's elements are consumed, so their counts go back to zero for the
    // next window to reuse.
    private static void ClearWindow(int[] nums, int start, int end, int[] windowFrequency)
    {
        var n = nums.Length;

        for (var i = start; i < end; i++)
        {
            var value = nums[i];

            if (value <= n)
            {
                windowFrequency[value] = 0;
            }
        }
    }

    private static int MexOf(int[] nums, int from, int to)
    {
        var present = new HashSet<int>();

        for (var i = from; i <= to; i++)
        {
            present.Add(nums[i]);
        }

        var mex = 0;

        while (present.Contains(mex))
        {
            mex++;
        }

        return mex;
    }
}
