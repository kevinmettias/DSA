using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.BuildAMatrixWithConditions;

internal readonly struct ValueTopology : IGraphTopology<ValueNode, ListChildren<ValueNode>>
{
    public static ListChildren<ValueNode> GetChildren(ValueNode node) => new(node.After);
}
