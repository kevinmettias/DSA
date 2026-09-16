namespace DSAExperimentation.LeetCode.LastRemainingIntegerAfterAlternatingDeletionOperations;

// LeetCode 3782. Last Remaining Integer After Alternating Deletion
// Operations: starting from [1..startingCount], alternately keep every second
// number counting from the left, then from the right, until one remains.
// startingCount reaches 10^15, so only the closed-form strategy is viable at
// LeetCode's own scale - no repo primitive applies to the arithmetic itself, the
// same "lighter repo-primitive fit" case EliminationGameTests' own doc comment
// already accepts for LC 390's closed-form solution (and Pow(x, n)/Rectangle Area
// before it).
internal static class LastRemainingIntegerAfterAlternatingDeletionOperationsSolution
{
    // The textbook simulation: materialize [1..startingCount] and repeatedly keep
    // every second element, alternating which end "every second" counts from. Only
    // sane up to a few million starting values - the arm the O(log n) strategy
    // below has to beat, and the one this problem's own 10^15 upper bound rules out
    // at full scale.
    public static long FindLastRemainingByListSimulation(long startingCount)
    {
        var current = new List<long>();

        for (var value = 1L; value <= startingCount; value++)
        {
            current.Add(value);
        }

        var side = PassDirection.Left;

        while (current.Count > 1)
        {
            current = KeepEverySecond(current, side);
            side = side == PassDirection.Left ? PassDirection.Right : PassDirection.Left;
        }

        return current[0];
    }

    private static List<long> KeepEverySecond(List<long> values, PassDirection side)
    {
        var kept = new List<long>();

        if (side == PassDirection.Left)
        {
            for (var i = 0; i < values.Count; i += 2)
            {
                kept.Add(values[i]);
            }

            return kept;
        }

        for (var i = values.Count - 1; i >= 0; i -= 2)
        {
            kept.Add(values[i]);
        }

        kept.Reverse();
        return kept;
    }

    // Tracks the surviving run as an arithmetic sequence (head, step,
    // remaining count) instead of materializing it. Every pass keeps exactly
    // ceil(remaining / 2) elements and doubles step; only a right-to-left
    // pass on an even-count run shifts head, because that is the only case
    // where the kept run's new first element isn't the old run's first
    // element (a right-to-left pass on an odd-count run, or any left-to-
    // right pass, always keeps the run's existing first term). O(log n).
    public static long FindLastRemainingByHeadStepSimulation(long startingCount)
    {
        var head = 1L;
        var step = 1L;
        var remaining = startingCount;
        var side = PassDirection.Left;

        while (remaining > 1)
        {
            if (side == PassDirection.Right && remaining % 2 == 0)
            {
                head += step;
            }

            remaining = (remaining + 1) / 2;
            step *= 2;
            side = side == PassDirection.Left ? PassDirection.Right : PassDirection.Left;
        }

        return head;
    }

    // The end a pass counts "every second" from, which is also the direction it
    // sweeps in: a Left pass keeps indices 0, 2, 4, ... straight off the run, a
    // Right pass keeps the same ordinals counted back from the end. The two
    // strategies alternate it once per pass.
    private enum PassDirection
    {
        Left,
        Right,
    }
}
