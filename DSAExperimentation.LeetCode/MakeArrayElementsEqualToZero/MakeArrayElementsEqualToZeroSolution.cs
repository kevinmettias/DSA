namespace DSAExperimentation.LeetCode.MakeArrayElementsEqualToZero;

// LeetCode 3354. Make Array Elements Equal to Zero: pick a zero index and a
// direction; a walker steps through the array, passing through zero cells
// untouched but decrementing and turning around at every nonzero cell it
// lands on, until it steps out of bounds. Count the (index, direction) pairs
// whose walk leaves every element zero.
internal static class MakeArrayElementsEqualToZeroSolution
{
    // The textbook baseline: actually run the walk the problem describes,
    // once per zero index per direction, on a scratch copy of nums - the arm
    // the prefix-sum strategy below has to justify itself against.
    public static int CountValidSelectionsByBruteForceSimulation(int[] nums)
    {
        var count = 0;

        for (var start = 0; start < nums.Length; start++)
        {
            if (nums[start] != 0)
            {
                continue;
            }

            if (IsClearedByWalk(nums, start, direction: -1))
            {
                count++;
            }

            if (IsClearedByWalk(nums, start, direction: 1))
            {
                count++;
            }
        }

        return count;
    }

    private static bool IsClearedByWalk(int[] nums, int start, int direction)
    {
        var scratch = (int[])nums.Clone();
        var curr = start;

        while (curr >= 0 && curr < scratch.Length)
        {
            if (scratch[curr] == 0)
            {
                curr += direction;
                continue;
            }

            scratch[curr]--;
            direction = -direction;
            curr += direction;
        }

        return Array.TrueForAll(scratch, value => value == 0);
    }

    // A walk that starts at a zero bounces between the mass to its left and
    // the mass to its right, chipping one unit off whichever side it lands
    // on next, so it can only ever finish both sides at zero when the two
    // sides are already balanced: equal (either direction empties both) or
    // exactly 1 apart (only the heavier side's direction empties both).
    public static int CountValidSelectionsByPrefixSumBalance(int[] nums)
    {
        var total = SumAll(nums);

        return CountSelections(nums, total);
    }

    // The array's total mass - the right-hand side of every balance check.
    private static int SumAll(int[] nums)
    {
        var total = 0;

        foreach (var value in nums)
        {
            total += value;
        }

        return total;
    }

    // Sweep left to right, tracking the mass already passed, and add up the
    // directions from each zero that would leave both sides empty.
    private static int CountSelections(int[] nums, int total)
    {
        var count = 0;
        var left = 0;

        foreach (var value in nums)
        {
            if (value != 0)
            {
                left += value;
                continue;
            }

            count += CountDirectionsFromZero(left, total - left);
        }

        return count;
    }

    // Equal sides empty under either direction; sides exactly 1 apart empty only
    // under the heavier side's direction; anything else empties under neither.
    private static int CountDirectionsFromZero(int left, int right)
    {
        if (left == right)
        {
            return 2;
        }

        if (Math.Abs(left - right) == 1)
        {
            return 1;
        }

        return 0;
    }
}
