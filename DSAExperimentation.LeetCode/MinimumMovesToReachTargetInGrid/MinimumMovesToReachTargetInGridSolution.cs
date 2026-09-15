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
            var node = queue.Dequeue();

            if (node.X == tx && node.Y == ty)
            {
                return node.Moves;
            }

            if (node.X == 0 && node.Y == 0)
            {
                continue;
            }

            EnqueueMoves(queue, node, tx, ty);
        }

        return LeetCodeAnswer.None;
    }

    // Queues both moves available from `node`, skipping any that would overshoot the
    // target - a coordinate only ever grows, so an overshoot can never recover.
    private static void EnqueueMoves(
        Queue<(long X, long Y, int Moves)> queue, (long X, long Y, int Moves) node, int tx, int ty)
    {
        var m = Math.Max(node.X, node.Y);

        if (node.X + m <= tx && node.Y <= ty)
        {
            queue.Enqueue((node.X + m, node.Y, node.Moves + 1));
        }

        if (node.X <= tx && node.Y + m <= ty)
        {
            queue.Enqueue((node.X, node.Y + m, node.Moves + 1));
        }
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
            if (!TryStepBack(x, y, ref targetX, ref targetY))
            {
                return LeetCodeAnswer.None;
            }

            moves++;
        }

        return moves;
    }

    // Undoes the single forced last move, reporting false once the walk reaches a state
    // no move could have produced.
    private static bool TryStepBack(long x, long y, ref long targetX, ref long targetY)
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
                return false;
            }
        }
        else if (targetX > targetY)
        {
            if (!TryReduceLarger(ref targetX, targetY))
            {
                return false;
            }
        }
        else if (!TryReduceLarger(ref targetY, targetX))
        {
            return false;
        }

        return true;
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
