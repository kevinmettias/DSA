using DSAExperimentation.DataStructures.Graph.ShortestPaths;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.IPO;

// LeetCode 502. IPO: the classic two-heap greedy. A min-heap of projects ordered by
// required capital (this repo's Heap<(int Node, int Priority), ByPriorityOrder
// <int,int>> - the same (node, priority) projection TopKFrequentElementsTests uses
// for Dijkstra/A*'s frontier, here projecting (profit, capital) instead of
// (node, distance)) unlocks newly-affordable projects into a max-heap of their
// profits (Heap<int, MaxHeapOrder<int>>, the same size-limited-heap shape
// KthLargestElementTests uses with MinHeapOrder). Each of the k rounds moves every
// now-affordable project's profit into the max-heap, then greedily takes the best
// one, growing capital for the next round.
public sealed partial class IPOTests
{
    [Fact]
    public void FindMaximizedCapital_ClassicExample_GreedilyPicksBestAffordableProjects()
    {
        int[] profits = [1, 2, 3];
        int[] capitals = [0, 1, 1];

        var result = FindMaximizedCapital(k: 2, w: 0, profits, capitals);

        Assert.Equal(4, result);
    }

    [Fact]
    public void FindMaximizedCapital_SingleRound_UnlocksOnlyOneProfitBoost()
    {
        int[] profits = [1, 2, 3];
        int[] capitals = [0, 1, 2];

        var result = FindMaximizedCapital(k: 1, w: 0, profits, capitals);

        Assert.Equal(1, result);
    }

    [Fact]
    public void FindMaximizedCapital_NoAffordableProject_ReturnsStartingCapital()
    {
        int[] profits = [5];
        int[] capitals = [10];

        var result = FindMaximizedCapital(k: 3, w: 0, profits, capitals);

        Assert.Equal(0, result);
    }

    private static int FindMaximizedCapital(int k, int w, int[] profits, int[] capitals)
    {
        var byCapital = new Heap<(int Node, int Priority), ByPriorityOrder<int, int>>();

        for (var i = 0; i < profits.Length; i++)
        {
            byCapital.Push((profits[i], capitals[i]));
        }

        var byProfit = new Heap<int, MaxHeapOrder<int>>();

        for (var round = 0; round < k; round++)
        {
            while (byCapital.TryPeek(out var cheapest) && cheapest.Priority <= w)
            {
                byCapital.TryPop(out var popped);
                byProfit.Push(popped.Node);
            }

            if (!byProfit.TryPop(out var bestProfit))
            {
                break;
            }

            w += bestProfit;
        }

        return w;
    }
}
