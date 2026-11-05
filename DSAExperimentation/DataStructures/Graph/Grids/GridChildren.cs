using System.Numerics;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.DataStructures.Graph.Grids;

// A cell's open neighbours along its board's steps (Grid.Steps: the four orthogonal ones unless the
// board was given others), in that table's order, filtered to the ones the board says are in bounds
// and passable. TNodes says which node a neighbour is: the board's one node for its cell, or a new one.
//
// Which steps lead somewhere open is read once, when the children are made, into a mask with one bit
// per step, so Count is the mask's population count and Get(i) the step of its i-th set bit. The
// engines ask Count on every turn of their child loop and then Get each index; scanning the steps for
// each of those asked an orthogonal board about 14 times a node, and an octilinear one about 44, where
// the mask asks once per step.
/// <summary>
/// A cell's open neighbours along its board's steps, read from the board once, in the steps' order.
/// </summary>
/// <typeparam name="TNodes">Which node each neighbour is handed as.</typeparam>
public readonly struct GridChildren<TNodes> : IChildren<GridNode>
    where TNodes : struct, IGridNodeSource
{
    private readonly GridNode _node;

    // Bit i is set when the board's step i leads to a cell a walk may enter.
    private readonly ulong _openSteps;

    /// <inheritdoc/>
    public int Count => BitOperations.PopCount(_openSteps);

    /// <summary>Reads which of a cell's neighbours are open.</summary>
    /// <param name="node">The cell whose neighbours to list.</param>
    public GridChildren(GridNode node)
    {
        _node = node;
        _openSteps = OpenStepsOf(node);
    }

    /// <inheritdoc/>
    public GridNode Get(int index)
    {
        var remaining = _openSteps;

        for (var skipped = 0; skipped < index; skipped++)
        {
            remaining &= remaining - 1;
        }

        if (remaining == 0)
        {
            throw new IndexOutOfRangeException();
        }

        var (dRow, dCol) = _node.Grid.Steps[BitOperations.TrailingZeroCount(remaining)];
        return TNodes.NodeAt(_node.Grid, _node.Row + dRow, _node.Col + dCol);
    }

    private static ulong OpenStepsOf(GridNode node)
    {
        var steps = node.Grid.Steps;
        var open = 0ul;

        for (var step = 0; step < steps.Length; step++)
        {
            var (dRow, dCol) = steps[step];

            if (node.Grid.IsPassable(node.Row + dRow, node.Col + dCol))
            {
                open |= 1ul << step;
            }
        }

        return open;
    }
}
