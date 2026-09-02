namespace DSAExperimentation.LeetCode.MinimumMovesToReachTargetInGrid;

// LeetCode 3609. Minimum Moves to Reach Target in Grid: from (x, y), with
// m = max(x, y), a move goes to (x + m, y) or (x, y + m) - so along any path from
// the start both coordinates only ever grow, and the state space explored forward is
// unbounded for a target far from the start.
//
// Working backward from the target instead, the *last* move is forced whenever
// tx != ty: adding to x always leaves new_x >= new_y (with equality only when the
// pre-move x was 0), and adding to y is the mirror image, so only one of the two
// moves can ever have produced a state with unequal coordinates. That determines the
// predecessor up to a single ambiguous choice at tx == ty > 0, where the move could
// have come from either axis - resolved by which one of sx/sy is 0, since once a
// coordinate is driven to 0 on the way back it stays 0 for the rest of the walk.
internal static class MinimumMovesToReachTargetInGridSolution
{
    // The textbook answer: forward BFS, pruned the instant either coordinate would
    // overshoot the target (coordinates never shrink, so an overshoot can never
    // recover). Deliberately written without this repo's primitives, and only
    // tractable for the small gaps LeetCode's own examples use - the arm the
    // composed backward reduction below has to justify itself against for anything
    // larger.
    public static int MinMovesByBoundedForwardBfs(int sx, int sy, int tx, int ty)
    {
        var queue = new Queue<(long X, long Y, int Moves)>();
        queue.Enqueue((sx, sy, 0));

        while (queue.Count > 0)
        {
            var (x, y, moves) = queue.Dequeue();

            if (x == tx && y == ty)
            {
                return moves;
            }

            if (x == 0 && y == 0)
            {
                continue;
            }

            var m = Math.Max(x, y);

            if (x + m <= tx && y <= ty)
            {
                queue.Enqueue((x + m, y, moves + 1));
            }

            if (x <= tx && y + m <= ty)
            {
                queue.Enqueue((x, y + m, moves + 1));
            }
        }

        return LeetCodeAnswer.None;
    }

    // Backward reduction: undo the forced last move one step at a time. Each step
    // strictly shrinks the coordinate sum, and the halving case (below) collapses a
    // whole run of same-direction moves into one division, so this terminates in
    // O(log(max coordinate)) steps - the same halve-or-subtract shape as the
    // Euclidean algorithm.
    public static int MinMovesByBackwardReduction(int sx, int sy, int tx, int ty)
    {
        long x = sx, y = sy, targetX = tx, targetY = ty;
        var moves = 0;

        while (targetX != x || targetY != y)
        {
            if (targetX == targetY)
            {
                if (x == 0)
                {
                    targetX = 0;
                }
                else if (y == 0)
                {
                    targetY = 0;
                }
                else
                {
                    return LeetCodeAnswer.None;
                }
            }
            else if (targetX > targetY)
            {
                if (!TryReduceLarger(ref targetX, targetY))
                {
                    return LeetCodeAnswer.None;
                }
            }
            else if (!TryReduceLarger(ref targetY, targetX))
            {
                return LeetCodeAnswer.None;
            }

            moves++;
        }

        return moves;
    }

    // Undoes whichever move most recently grew `larger` past `smaller`: halve it
    // when it is at least double (the pre-move x >= y case, where the move doubled
    // it), otherwise subtract the other coordinate once (the pre-move x < y case) -
    // the two mutually exclusive predecessors of a single forced move.
    private static bool TryReduceLarger(ref long larger, long smaller)
    {
        if (larger >= 2 * smaller)
        {
            if (larger % 2 != 0)
            {
                return false;
            }

            larger /= 2;
        }
        else
        {
            larger -= smaller;
        }

        return true;
    }
}
