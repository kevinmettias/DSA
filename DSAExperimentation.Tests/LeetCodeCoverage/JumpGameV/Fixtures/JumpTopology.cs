using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Tests.LeetCodeCoverage.JumpGameV.Fixtures;

internal readonly struct JumpTopology : IGraphTopology<JumpNode, ListChildren<JumpNode>>
{
    public static ListChildren<JumpNode> GetChildren(JumpNode node) => new(node.ReachableIndices);
}
