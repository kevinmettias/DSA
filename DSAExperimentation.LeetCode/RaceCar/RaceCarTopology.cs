using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.RaceCar;

// Adjacency over the command graph: a state's children are exactly the states one
// 'A' or 'R' command away, already materialized on the node by RaceCarStateGraph.
internal readonly struct RaceCarTopology : IGraphTopology<RaceCarNode, ListChildren<RaceCarNode>>
{
    public static ListChildren<RaceCarNode> GetChildren(RaceCarNode node) => new(node.Neighbors);
}
