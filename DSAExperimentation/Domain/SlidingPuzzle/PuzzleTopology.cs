using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Domain.SlidingPuzzle;

/// <summary>The graph topology over puzzle boards: a board's children are its blank-slide neighbors.</summary>
public readonly struct PuzzleTopology : IGraphTopology<PuzzleNode, ListChildren<PuzzleNode>>
{
    public static ListChildren<PuzzleNode> GetChildren(PuzzleNode node) => new(node.Neighbors);
}
