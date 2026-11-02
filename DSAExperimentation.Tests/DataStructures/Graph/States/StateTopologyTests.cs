using DSAExperimentation.DataStructures.Graph.States;

namespace DSAExperimentation.Tests.DataStructures.Graph.States;

public sealed partial class StateTopologyTests
{
    [Fact]
    public void GetChildren_AreTheNodesSuccessors()
    {
        var node = new StateNode<int>(0);
        var next = new StateNode<int>(1);
        node.Successors.Add(next);

        var children = StateTopology<int>.GetChildren(node);

        Assert.Equal(1, children.Count);
        Assert.Same(next, children.Get(0));
    }
}
