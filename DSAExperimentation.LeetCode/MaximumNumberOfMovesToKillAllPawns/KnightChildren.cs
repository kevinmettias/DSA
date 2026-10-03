using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.LeetCode.MaximumNumberOfMovesToKillAllPawns;

// Knight-move adjacency for LC 3283 alone: GridChildren's own 4-orthogonal
// offsets don't fit a knight, so this problem gets its own IChildren witness over
// the very same GridNode/Grid pair GridChildren already wraps, swapping in the 8
// L-shaped knight offsets instead. The board has no obstacles - a knight passes
// over other pawns freely, per the problem statement - so Grid's plain bounds
// check is all IsPassable needs to do here.
internal readonly struct KnightChildren(GridNode node) : IChildren<GridNode>
{
    public int Count
    {
        get
        {
            var count = 0;

            for (var i = 0; i < GridDirections.Knight.Length; i++)
            {
                if (IsOnBoard(i))
                {
                    count++;
                }
            }

            return count;
        }
    }

    public GridNode Get(int index)
    {
        for (var i = 0; i < GridDirections.Knight.Length; i++)
        {
            if (IsOnBoard(i))
            {
                if (index == 0)
                {
                    var (deltaRow, deltaCol) = GridDirections.Knight[i];
                    return new GridNode(node.Row + deltaRow, node.Col + deltaCol, node.Grid);
                }

                index--;
            }
        }

        throw new IndexOutOfRangeException();
    }

    private bool IsOnBoard(int offset)
    {
        var (deltaRow, deltaCol) = GridDirections.Knight[offset];
        return node.Grid.IsPassable(node.Row + deltaRow, node.Col + deltaCol);
    }
}
