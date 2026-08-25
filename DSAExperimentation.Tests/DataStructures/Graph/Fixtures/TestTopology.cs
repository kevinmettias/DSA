using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

internal readonly struct TestTopology : ITreeTopology<TestNode, ListChildren<TestNode>>
{
    public static ListChildren<TestNode> GetChildren(TestNode node) => new(node.Children);
}
