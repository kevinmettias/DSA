using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.DataStructures.Graph.Grids;

// Computed on demand from geometry, the same way SparseArrayChildren scans slots
// instead of storing a materialized list - up to 4 orthogonal neighbors, filtered to
// the ones the node's Grid says are in bounds and passable, in GridDirections.Orthogonal's
// order. TNodes says which node a neighbour is: the board's one node for its cell, or a new one.
/// <summary>
/// A cell's open orthogonal neighbours, computed from its board on each access in up, down, left, right order.
/// </summary>
/// <typeparam name="TNodes">Which node each neighbour is handed as.</typeparam>
/// <param name="node">The cell whose neighbours to list.</param>
public readonly struct GridChildren<TNodes>(GridNode node) : IChildren<GridNode>
    where TNodes : struct, IGridNodeSource
{
    public int Count
    {
        get
        {
            var count = 0;

            for (var i = 0; i < GridDirections.Orthogonal.Length; i++)
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
        for (var i = 0; i < GridDirections.Orthogonal.Length; i++)
        {
            if (IsPassableNeighbor(i))
            {
                if (index == 0)
                {
                    var (dRow, dCol) = GridDirections.Orthogonal[i];
                    return TNodes.NodeAt(node.Grid, node.Row + dRow, node.Col + dCol);
                }

                index--;
            }
        }

        throw new IndexOutOfRangeException();
    }

    private bool IsPassableNeighbor(int direction)
    {
        var (dRow, dCol) = GridDirections.Orthogonal[direction];
        return node.Grid.IsPassable(node.Row + dRow, node.Col + dCol);
    }
}
