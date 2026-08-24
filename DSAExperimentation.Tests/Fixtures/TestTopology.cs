using DSAExperimentation.Graph.Contracts.Ordering;
using DSAExperimentation.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.Fixtures;

internal readonly struct TestTopology : ITreeTopology<TestNode, ListChildren<TestNode>>
{
    public static ListChildren<TestNode> GetChildren(TestNode node) => new(node.Children);
}
