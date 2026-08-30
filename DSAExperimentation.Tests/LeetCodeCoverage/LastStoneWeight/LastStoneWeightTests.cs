using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LastStoneWeight;

// LeetCode 1046. Last Stone Weight: repeatedly smash the two heaviest stones
// together, using this repo's own Heap<int,MaxHeapOrder<int>> to always offer up
// the two heaviest remaining stones in O(log n) instead of an O(n) full rescan
// every smash.
public sealed partial class LastStoneWeightTests
{
    [Fact]
    public void LastStoneWeight_ClassicExample_ReturnsOne()
    {
        int[] stones = [2, 7, 4, 1, 8, 1];

        Assert.Equal(1, LastStoneWeight(stones));
    }

    [Fact]
    public void LastStoneWeight_AllStonesCancelOut_ReturnsZero()
    {
        int[] stones = [2, 2];

        Assert.Equal(0, LastStoneWeight(stones));
    }

    [Fact]
    public void LastStoneWeight_SingleStone_ReturnsItsWeight()
    {
        int[] stones = [5];

        Assert.Equal(5, LastStoneWeight(stones));
    }

    private static int LastStoneWeight(int[] stones)
    {
        var heap = new Heap<int, MaxHeapOrder<int>>();

        foreach (var stone in stones)
        {
            heap.Push(stone);
        }

        while (heap.Count > 1)
        {
            heap.TryPop(out var heaviest);
            heap.TryPop(out var second);

            if (heaviest != second)
            {
                heap.Push(heaviest - second);
            }
        }

        heap.TryPeek(out var remaining);
        return heap.Count == 0 ? 0 : remaining;
    }
}
