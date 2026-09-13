namespace DSAExperimentation.LeetCode.MinimumNumberOfIncrementsOnSubarraysToFormTargetArray;

// LeetCode 1526. Minimum Number of Increments on Subarrays to Form a Target Array: each
// operation increments one contiguous subarray by 1, starting from an all-zero array; report
// the fewest operations that reach target.
//
// The two strategies differ in whether they simulate the operations or reason about them. The
// baseline literally performs them, layer by layer, each pass extending one stroke as far right
// as it can before starting the next - O(n * max(target)). The composed strategy observes that
// a stroke can only begin where the array rises, so the answer is exactly the sum of positive
// rises between consecutive elements (target[0] counting as a rise from an implicit leading 0),
// a single O(n) pass. No repo primitive applies to either side - both are pure array scans, the
// same category MaximumSubarray and JumpGame already established for this kind of greedy
// single-pass array problem.
internal static class MinimumNumberOfIncrementsOnSubarraysToFormTargetArraySolution
{
    // Baseline: actually run the operations. Each sweep walks left to right, starting a stroke
    // wherever the working array is still below target and carrying it right until it is not.
    public static int MinNumberOperationsByLayerSimulation(int[] target)
    {
        var current = new int[target.Length];
        var operations = 0;
        var madeProgress = true;

        while (madeProgress)
        {
            (madeProgress, operations) = RunSweep(current, target, operations);
        }

        return operations;
    }

    // One left-to-right pass of the simulation, applying every stroke that fits in this layer.
    private static (bool MadeProgress, int Operations) RunSweep(int[] current, int[] target, int operations)
    {
        var madeProgress = false;
        var i = 0;

        while (i < current.Length)
        {
            var (nextIndex, advancedStroke) = AdvanceFromIndex(current, target, i);
            i = nextIndex;

            if (advancedStroke)
            {
                madeProgress = true;
                operations++;
            }
        }

        return (madeProgress, operations);
    }

    // Extend a single stroke from index as far right as the target still allows, reporting where
    // the sweep should resume and whether a stroke was actually laid down.
    private static (int Index, bool AdvancedStroke) AdvanceFromIndex(int[] current, int[] target, int index)
    {
        if (current[index] >= target[index])
        {
            return (index + 1, false);
        }

        while (index < current.Length && current[index] < target[index])
        {
            current[index]++;
            index++;
        }

        return (index, true);
    }

    // Sum the positive rises between consecutive elements: every unit of rise is a stroke that
    // has to start at that index, and every fall is a stroke that simply ends earlier.
    public static int MinNumberOperationsByRisingDiffScan(int[] target)
    {
        var operations = 0;
        var previous = 0;

        foreach (var level in target)
        {
            if (level > previous)
            {
                operations += level - previous;
            }

            previous = level;
        }

        return operations;
    }
}
