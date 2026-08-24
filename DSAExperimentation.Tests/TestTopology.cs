using DSAExperimentation.Graph;

namespace DSAExperimentation.Tests;

public readonly struct TestTopology : ITreeTopology<TestNode, ListChildren<TestNode>>
{
    public static ListChildren<TestNode> GetChildren(TestNode node) => new(node.Children);
}
