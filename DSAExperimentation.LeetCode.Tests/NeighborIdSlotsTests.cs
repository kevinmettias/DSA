namespace DSAExperimentation.LeetCode.Tests;

public sealed partial class NeighborIdSlotsTests
{
    [Fact]
    public void SlotFor_AnyNode_IsAnEmptyNeighbourList() => Assert.Empty(new NeighborIdSlots().SlotFor(3));

    // One endpoint's view of an edge: the far id goes on the end of this slot's list, and the far
    // slot is left for its own call.
    [Fact]
    public void Wire_AppendsTheFarIdToItsOwnSlotOnly()
    {
        var slot = new List<int> { 4 };
        var farSlot = new List<int>();

        new NeighborIdSlots().Wire(slot, 7, farSlot, 0);

        Assert.Equal([4, 7], slot);
        Assert.Empty(farSlot);
    }
}
