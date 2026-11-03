namespace DSAExperimentation.LeetCode.Tests;

public sealed partial class WeightedNeighborSlotsTests
{
    private static readonly int[][] Edges = [[0, 1, 5], [1, 2, 9]];

    [Fact]
    public void SlotFor_AnyNode_IsAnEmptyNeighbourList() => Assert.Empty(new WeightedNeighborSlots<int>(Edges).SlotFor(1));

    // The weight is read from the third column of the edge the index names, not from either id.
    [Fact]
    public void Wire_AppendsTheFarIdBesideThatEdgesWeight()
    {
        var slot = new List<(int To, int Weight)>();
        var farSlot = new List<(int To, int Weight)>();

        new WeightedNeighborSlots<int>(Edges).Wire(slot, 2, farSlot, 1);

        Assert.Equal([(2, 9)], slot);
        Assert.Empty(farSlot);
    }

    // A long slot reads the same column, widened, so a sum of such weights cannot overflow int.
    [Fact]
    public void Wire_LongWeights_ReadsTheSameWeightWidened()
    {
        var slot = new List<(int To, long Weight)>();

        new WeightedNeighborSlots<long>([[0, 1, int.MaxValue]]).Wire(slot, 1, [], 0);

        Assert.Equal([(1, (long)int.MaxValue)], slot);
    }

    [Fact]
    public void ZeroBased_WeightedSlots_GivesBothEndpointsTheEdgesWeight()
    {
        var neighbors = LeetCodeAdjacency.ZeroBased<List<(int To, int Weight)>, WeightedNeighborSlots<int>>(3, Edges, new WeightedNeighborSlots<int>(Edges));

        Assert.Equal([(1, 5)], neighbors[0]);
        Assert.Equal([(0, 5), (2, 9)], neighbors[1]);
        Assert.Equal([(1, 9)], neighbors[2]);
    }
}
