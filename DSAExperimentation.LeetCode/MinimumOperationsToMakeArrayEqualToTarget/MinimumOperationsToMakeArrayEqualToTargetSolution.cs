namespace DSAExperimentation.LeetCode.MinimumOperationsToMakeArrayEqualToTarget;

// LeetCode 3229. Minimum Operations to Make Array Equal to Target: an operation picks
// any subarray of nums and adds +1 or -1 to every element in it; find the fewest
// operations to turn nums into target.
//
// Both strategies work on diff[i] = target[i] - nums[i], the amount still owed at each
// position. Neither composes a data structure - the optimal answer is a single O(n)
// scan over plain arrays, so there is no existing repo primitive whose job this is;
// forcing one in (a monotonic Stack<T>, a RangeFenwickTree) would either misrepresent
// the algorithm or actively compute the wrong answer, not just look unnecessary. See
// MinOperationsByDifferenceScan's own comment for why.
internal static class MinimumOperationsToMakeArrayEqualToTargetSolution
{
    // The literal textbook approach: repeatedly find the leftmost position that still
    // disagrees with target, extend the widest contiguous run from there that all needs
    // the same direction of nudge, and apply one unit of +1/-1 across that whole run -
    // exactly what the problem statement describes an "operation" as. Provably optimal
    // (each step commits to the largest possible run), but O(operations * n) because it
    // rescans from index 0 and re-walks the run on every single unit of every operation,
    // where the scan-based strategy below does the same job in one O(n) pass.
    public static long MinOperationsByBruteForceSimulation(int[] nums, int[] target)
    {
        var current = (int[])nums.Clone();
        var operations = 0L;

        // Stops once no position still disagrees with target: FindFirstMismatch returns -1 and the operation count is returned.
        while (true)
        {
            var start = FindFirstMismatch(current, target);

            if (start == -1)
            {
                return operations;
            }

            ApplyWidestRun(current, target, start);
            operations++;
        }
    }

    private static int FindFirstMismatch(int[] current, int[] target)
    {
        for (var i = 0; i < current.Length; i++)
        {
            if (current[i] != target[i])
            {
                return i;
            }
        }

        return -1;
    }

    // One unit of one operation: from `start`, the widest contiguous run that all needs
    // the same direction of nudge, every element of it shifted by that one unit.
    private static void ApplyWidestRun(int[] current, int[] target, int start)
    {
        var direction = Math.Sign(target[start] - current[start]);
        var end = FindRunEnd(current, target, start, direction);

        for (var i = start; i <= end; i++)
        {
            current[i] += direction;
        }
    }

    private static int FindRunEnd(int[] current, int[] target, int start, int direction)
    {
        var end = start;

        while (end + 1 < current.Length && Math.Sign(target[end + 1] - current[end + 1]) == direction)
        {
            end++;
        }

        return end;
    }

    // A subarray operation changes diff[l] and diff[r+1] by one unit each, in opposite
    // directions - the same edge-pair shape a difference array uses for range updates.
    // Realizing a target diff[] sequence from all-zero therefore costs exactly the sum
    // of its positive steps (with virtual diff[-1] = diff[n] = 0 sentinels): every
    // positive step must be paid for by opening that many new operations, and every
    // negative step is free because it only closes operations already paid for. This is
    // the general (both-directions) form of LC 1526's range-increment identity, and it
    // is the whole algorithm - one running "previous diff" variable, no data structure,
    // repo or otherwise, has anything to add to a single O(n) pass over two arrays.
    public static long MinOperationsByDifferenceScan(int[] nums, int[] target)
    {
        var operations = 0L;
        var previousDiff = 0;

        for (var i = 0; i < nums.Length; i++)
        {
            var diff = target[i] - nums[i];
            operations += Math.Max(0, diff - previousDiff);
            previousDiff = diff;
        }

        operations += Math.Max(0, -previousDiff);
        return operations;
    }
}
