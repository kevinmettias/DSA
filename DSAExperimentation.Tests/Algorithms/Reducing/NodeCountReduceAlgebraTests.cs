using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Reducing;

public sealed partial class NodeCountReduceAlgebraTests
{
    [Fact]
    public void Seed_IsZero() => Assert.Equal(0, NodeCountReduceAlgebra<TestNode>.Seed);

    public static TheoryData<int> Depths => new() { 0, 7 };

    // The node and its depth say nothing about the count: any node at any depth adds one.
    [Theory]
    [MemberData(nameof(Depths))]
    public void Enter_AddsOneWhateverTheDepth(int depth)
        => Assert.Equal(5, NodeCountReduceAlgebra<TestNode>.Enter(4, new TestNode("A"), depth));
}
