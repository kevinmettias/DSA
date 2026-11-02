using DSAExperimentation.DataStructures.Graph.States;

namespace DSAExperimentation.Tests.DataStructures.Graph.States;

public sealed partial class StateNodeTests
{
    [Fact]
    public void State_IsTheStateTheNodeWasMadeFor() => Assert.Equal("0000", new StateNode<string>("0000").State);

    [Fact]
    public void Successors_StartEmpty() => Assert.Empty(new StateNode<string>("0000").Successors);

    [Fact]
    public void ToString_SpellsTheState() => Assert.Equal("0000", new StateNode<string>("0000").ToString());
}
