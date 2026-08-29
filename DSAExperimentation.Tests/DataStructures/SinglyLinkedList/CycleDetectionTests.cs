using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.Tests.DataStructures.SinglyLinkedList.Fixtures;

namespace DSAExperimentation.Tests.DataStructures.SinglyLinkedList;

public sealed partial class CycleDetectionTests
{
    [Fact]
    public void HasCycle_AcyclicList_ReturnsFalse()
        => Assert.False(CycleDetection.HasCycle(SinglyLinkedLists.Acyclic()));

    [Fact]
    public void HasCycle_NullHead_ReturnsFalse()
        => Assert.False(CycleDetection.HasCycle<int>(null));

    [Fact]
    public void HasCycle_ListWithCycle_ReturnsTrue()
    {
        var (head, _) = SinglyLinkedLists.WithCycle();

        Assert.True(CycleDetection.HasCycle(head));
    }

    [Fact]
    public void HasCycle_SingleNodeSelfCycle_ReturnsTrue()
        => Assert.True(CycleDetection.HasCycle(SinglyLinkedLists.SingleNodeSelfCycle()));

    [Fact]
    public void FindCycleStart_AcyclicList_ReturnsNull()
        => Assert.Null(CycleDetection.FindCycleStart(SinglyLinkedLists.Acyclic()));

    [Fact]
    public void FindCycleStart_NullHead_ReturnsNull()
        => Assert.Null(CycleDetection.FindCycleStart<int>(null));

    [Fact]
    public void FindCycleStart_ListWithCycle_ReturnsTheCycleEntryNode()
    {
        var (head, cycleStart) = SinglyLinkedLists.WithCycle();

        var found = CycleDetection.FindCycleStart(head);

        Assert.Same(cycleStart, found);
    }

    [Fact]
    public void FindCycleStart_SingleNodeSelfCycle_ReturnsThatNode()
    {
        var node = SinglyLinkedLists.SingleNodeSelfCycle();

        Assert.Same(node, CycleDetection.FindCycleStart(node));
    }
}
