using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.RemoveBoxes;

// LeetCode 546. Remove Boxes: interval DP over (left, right, extra) triples - extra
// counts same-colored boxes already known to sit just past right, so removing
// boxes[right] together with them scores (extra+1)^2, or boxes[right] can be
// deferred by first clearing out some inner sub-range so an earlier same-colored
// box at i becomes adjacent to it, growing its own extra count by one.
//
// Both strategies walk the same recurrence over the same boxes array; they differ
// only in whether repeated (left, right, extra) triples are cached.
internal static class RemoveBoxesSolution
{
    // The textbook answer: plain exponential recursion over (left, right, extra)
    // triples, no caching - the same triple recurs across many different choices of
    // which inner sub-range gets cleared first. Deliberately written without this
    // repo's primitives; it is the arm the composed solution below has to justify
    // itself against.
    public static int MaxPointsByUnmemoizedRecursion(int[] boxes) => PointsUnmemoized(0, boxes.Length - 1, 0, boxes);

    // This repo's own Memoizer<TState,TResult> supplies the cache, keyed by the
    // (left, right, extra) triple, the same tuple-state shape BurstBalloonsSolution
    // uses for its (left, right) pair, extended with the one extra dimension this
    // puzzle needs.
    public static int MaxPointsByMemoizedRecursion(int[] boxes) =>
        Memoizer.Memoize<(int Left, int Right, int Extra), int>(
            (0, boxes.Length - 1, 0),
            new BoxRemovalOrder(boxes));

    // The rule, named: removing boxes[right] together with the `extra` same-colored
    // boxes already known to sit just past it scores (extra + 1)^2, or boxes[right] can
    // be deferred by first clearing out some inner sub-range so an earlier same-colored
    // box at i becomes adjacent to it, growing its own extra count by one. The boxes are
    // the whole of what the rule needs from its caller, so they are the constructor's
    // only input.
    private sealed class BoxRemovalOrder(int[] boxes)
        : IRecurrence<(int Left, int Right, int Extra), int>
    {
        public int Replay(
            (int Left, int Right, int Extra) state,
            IRecurrence<(int Left, int Right, int Extra), int> rest)
        {
            var (left, right, extra) = state;

            if (left > right)
            {
                return 0;
            }

            var best = rest.Replay((left, right - 1, 0), rest) + (extra + 1) * (extra + 1);

            for (var i = left; i < right; i++)
            {
                if (boxes[i] == boxes[right])
                {
                    best = Math.Max(
                        best,
                        rest.Replay((left, i, extra + 1), rest) + rest.Replay((i + 1, right - 1, 0), rest));
                }
            }

            return best;
        }
    }

    private static int PointsUnmemoized(int left, int right, int extra, int[] boxes)
    {
        if (left > right)
        {
            return 0;
        }

        var best = PointsUnmemoized(left, right - 1, 0, boxes) + (extra + 1) * (extra + 1);

        for (var i = left; i < right; i++)
        {
            if (boxes[i] == boxes[right])
            {
                best = Math.Max(
                    best,
                    PointsUnmemoized(left, i, extra + 1, boxes) + PointsUnmemoized(i + 1, right - 1, 0, boxes));
            }
        }

        return best;
    }
}
