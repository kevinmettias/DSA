using DSAExperimentation.Graph.Contracts.Ordering;

namespace DSAExperimentation.Graph.Algorithms.Grids;

// Computed on demand from geometry, the same way SparseArrayChildren scans slots
// instead of storing a materialized list - up to 4 orthogonal neighbors, filtered to
// the ones the node's Grid says are in bounds and passable.
internal readonly struct GridChildren(GridNode node) : IChildren<GridNode>
{
    private static readonly (int DRow, int DCol)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    public int Count
    {
        get
        {
            var count = 0;

            for (var i = 0; i < Directions.Length; i++)
            {
                if (IsPassableNeighbor(i))
                {
                    count++;
                }
            }

            return count;
        }
    }

    public GridNode Get(int index)
    {
        for (var i = 0; i < Directions.Length; i++)
        {
            if (IsPassableNeighbor(i))
            {
                if (index == 0)
                {
                    var (dRow, dCol) = Directions[i];
                    return new GridNode(node.Row + dRow, node.Col + dCol, node.Grid);
                }

                index--;
            }
        }

        throw new IndexOutOfRangeException();
    }

    private bool IsPassableNeighbor(int direction)
    {
        var (dRow, dCol) = Directions[direction];
        return node.Grid.IsPassable(node.Row + dRow, node.Col + dCol);
    }
}
