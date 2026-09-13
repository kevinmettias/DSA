using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.SmallestIntegerDivisibleByK;

// The repunit-remainder graph's shape rule, as an IGraphTopology witness: a node's
// children are the single successor RemainderGraph.Build wired into Neighbors.
internal readonly struct RemainderTopology : IGraphTopology<RemainderNode, ListChildren<RemainderNode>>
{
    public static ListChildren<RemainderNode> GetChildren(RemainderNode node) => new(node.Neighbors);
}
