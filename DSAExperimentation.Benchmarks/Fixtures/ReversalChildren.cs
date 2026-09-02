using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Mirrors DSAExperimentation.Tests' MinimumReverseOperations ReversalChildren
// fixture: computed on demand from the board's own window arithmetic, touching only
// the O(K) positions actually reachable in one reversal.
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

    private static int FirstMirror(PositionNode node)
    {
        var earliestStart = Math.Max(0, node.Position - node.Board.K + 1);
        return Mirror(node, earliestStart);
    }

    private static int LastMirror(PositionNode node)
    {
        var latestStart = Math.Min(node.Position, node.Board.Length - node.Board.K);
        return Mirror(node, latestStart);
    }

    private static int Mirror(PositionNode node, int windowStart)
        => (2 * windowStart) + node.Board.K - 1 - node.Position;
}
