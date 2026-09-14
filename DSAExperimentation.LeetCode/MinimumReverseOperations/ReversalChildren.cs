using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.MinimumReverseOperations;

// Computed on demand from the board's own arithmetic, the same way GridChildren
// computes its (at most 4) neighbors from geometry instead of storing a materialized
// list: reversing any size-K window [L, L+K-1] that contains this node's position
// sends it to that window's mirror, L + (L+K-1-position) = 2L+K-1-position. As the
// window start L ranges over every value that keeps the window in bounds and still
// covering position, the mirror sweeps every other position between FirstMirror and
// LastMirror inclusive - never the full [0, Length) range, so this only ever touches
// the O(K) positions actually reachable in one operation.
internal readonly struct ReversalChildren(PositionNode node) : IChildren<PositionNode>
{
    public int Count
    {
        get
        {
            var count = 0;

            for (var position = FirstMirror(node); position <= LastMirror(node); position += 2)
            {
                if (!node.Board.IsBanned(position))
                {
                    count++;
                }
            }

            return count;
        }
    }

    public PositionNode Get(int index)
    {
        for (var position = FirstMirror(node); position <= LastMirror(node); position += 2)
        {
            if (node.Board.IsBanned(position))
            {
                continue;
            }

            if (index == 0)
            {
                return new PositionNode(position, node.Board);
            }

            index--;
        }

        throw new IndexOutOfRangeException();
    }

    // The mirror when the covering window starts as early as possible: L = max(0,
    // position - K + 1), clamped so the window still starts at or after index 0.
    private static int FirstMirror(PositionNode node)
    {
        var earliestStart = Math.Max(0, node.Position - node.Board.K + 1);
        return Mirror(node, earliestStart);
    }

    // The mirror when the covering window starts as late as possible: L = min(position,
    // Length - K), clamped so the window still ends at or before the last index.
    private static int LastMirror(PositionNode node)
    {
        var latestStart = Math.Min(node.Position, node.Board.Length - node.Board.K);
        return Mirror(node, latestStart);
    }

    private static int Mirror(PositionNode node, int windowStart)
        => (2 * windowStart) + node.Board.K - 1 - node.Position;
}
