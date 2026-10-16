using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Domain.SlidingPuzzle;

public readonly struct PuzzleTopology : IGraphTopology<PuzzleNode, ListChildren<PuzzleNode>>
{
    public static ListChildren<PuzzleNode> GetChildren(PuzzleNode node) => new(node.Neighbors);
}
