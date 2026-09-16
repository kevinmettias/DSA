namespace DSAExperimentation.LeetCode.ReachingPoints;

// LeetCode 780. Reaching Points: starting from (sx, sy), a move replaces the pair with
// (x + y, y) or (x, x + y). Decide whether (tx, ty) is reachable.
//
// Both strategies work backward from the target, undoing whichever move grew the larger
// coordinate - forward search branches, backward reduction does not, because only one of
// the two predecessors can have non-negative coordinates. They differ only in step size:
// SubtractiveReduction undoes one growth step at a time, ModuloReduction jumps straight
// to the remainder. That is the same subtraction-vs-modulo contrast that separates the
// naive and fast forms of Euclid's algorithm.
//
// Once one coordinate has fallen to its source value the other can only be topped up in
// fixed increments, so the tail is a divisibility check rather than more searching.
//
// No repo container or algorithm primitive applies here - there is nothing to compose
// over two running integer pairs, the same "lighter repo-primitive fit" case Pow(x, n)'s
// exponentiation by squaring already is.
internal static class ReachingPointsSolution
{
    // Baseline: undo one growth step at a time, so the step count grows with the gap
    // between the coordinates. Deliberately BCL-only internals (§17.5).
    public static bool IsReachableBySubtractiveReduction(int sx, int sy, int tx, int ty)
    {
        while (tx > sx && ty > sy)
        {
            if (tx > ty)
            {
                tx -= ty;
            }
            else
            {
                ty -= tx;
            }
        }

        return IsReachableInFixedIncrements(sx, sy, tx, ty);
    }

    // Undo an entire run of same-direction growth steps in one remainder operation, so
    // the step count is Euclidean rather than proportional to the coordinate values.
    public static bool IsReachableByModuloReduction(int sx, int sy, int tx, int ty)
    {
        while (tx > sx && ty > sy)
        {
            if (tx > ty)
            {
                tx %= ty;
            }
            else
            {
                ty %= tx;
            }
        }

        return IsReachableInFixedIncrements(sx, sy, tx, ty);
    }

    // With one coordinate already at its source value, the other one only ever grew by
    // that fixed value, so reachability is a divisibility check.
    private static bool IsReachableInFixedIncrements(int sx, int sy, int tx, int ty)
    {
        if (tx == sx)
        {
            return ty >= sy && (ty - sy) % sx == 0;
        }

        if (ty == sy)
        {
            return tx >= sx && (tx - sx) % sy == 0;
        }

        return false;
    }
}
