namespace DSAExperimentation.LeetCode.CheckIfPointIsReachable;

// LeetCode 2543. Check if Point Is Reachable: starting from (1, 1), the allowed
// moves are (x, y) -> (x, x + y), (x, y) -> (x + y, y), (x, y) -> (2x, y) and
// (x, y) -> (x, 2y); decide whether (targetX, targetY) can be reached.
//
// The two doubling moves inject - or, read backwards, freely remove - any factor
// of two, while the two addition moves are exactly one step of the deterministic,
// reversible subtractive Euclidean algorithm on whatever is left, which only ever
// bottoms out at (1, 1) when that remainder is already 1. So the target is
// reachable iff gcd(targetX, targetY) is a power of two. The second strategy is
// that closed form; the first one searches the state space for the same answer.
//
// A reduction over two running integers has no repo container or algorithm
// primitive to compose over, which is the reasoning FindGreatestCommonDivisorOf-
// ArraySolution and XOfAKindInADeckOfCardsSolution already record for their own
// inline Euclidean helpers.
internal static class CheckIfPointIsReachableSolution
{
    // Both coordinates of the start point (1, 1).
    private const long Origin = 1;

    // The multiplier the (2x, y) and (x, 2y) moves apply.
    private const long DoublingFactor = 2;

    // The textbook answer: breadth-first search over every intermediate point
    // reachable from (1, 1), pruned by the target because no move ever shrinks a
    // coordinate - so any path that reaches the target stays inside the box it
    // spans, and the search is still complete. Deliberately written with BCL
    // Queue/HashSet: it is the arm the closed form below has to justify itself
    // against, and its cost - one visited state per lattice point in that box -
    // is the whole point of the comparison.
    public static bool IsReachableByBruteForceBfs(int targetX, int targetY)
    {
        var start = (X: Origin, Y: Origin);
        var visited = new HashSet<(long X, long Y)> { start };
        var queue = new Queue<(long X, long Y)>();
        queue.Enqueue(start);

        return SweepByBreadthFirstSearch(queue, visited, targetX, targetY);
    }

    // Drains the frontier: each point popped either answers (it is the target) or
    // expands into its four moves, so the search ends only when nothing new is left.
    private static bool SweepByBreadthFirstSearch(
        Queue<(long X, long Y)> queue, HashSet<(long X, long Y)> visited, int targetX, int targetY)
    {
        while (queue.Count > 0)
        {
            var point = queue.Dequeue();

            if (point.X == targetX && point.Y == targetY)
            {
                return true;
            }

            foreach (var next in Successors(point.X, point.Y))
            {
                if (IsNewStateWithinTarget(next, targetX, targetY, visited))
                {
                    queue.Enqueue(next);
                }
            }
        }

        return false;
    }

    // The four moves, in the order the puzzle states them.
    private static IEnumerable<(long X, long Y)> Successors(long x, long y)
    {
        yield return (x + y, y);
        yield return (x, x + y);
        yield return (DoublingFactor * x, y);
        yield return (x, DoublingFactor * y);
    }

    // A successor is worth queueing only when it stays inside the target's box -
    // no move ever shrinks a coordinate, so nothing outside it can lead there -
    // and the search has not already seen it.
    private static bool IsNewStateWithinTarget(
        (long X, long Y) next, int targetX, int targetY, HashSet<(long X, long Y)> visited)
        => next.X <= targetX && next.Y <= targetY && visited.Add(next);

    // The closed form: the gcd carries every factor the two coordinates share, and
    // only its odd part is an obstacle, so the point is reachable exactly when the
    // gcd is a power of two.
    public static bool IsReachableByGcd(int targetX, int targetY)
    {
        var gcd = EuclideanGcd(targetX, targetY);

        return IsPowerOfTwo(gcd);
    }

    // value & (value - 1) clears the lowest set bit, so only a value with a single
    // bit set leaves zero. Both coordinates are at least 1 (LC 2543's own bound),
    // so the gcd is too and the degenerate zero this trick also accepts cannot
    // arise.
    private static bool IsPowerOfTwo(int value) => (value & (value - 1)) == 0;

    private static int EuclideanGcd(int a, int b) => b == 0 ? a : EuclideanGcd(b, a % b);
}
