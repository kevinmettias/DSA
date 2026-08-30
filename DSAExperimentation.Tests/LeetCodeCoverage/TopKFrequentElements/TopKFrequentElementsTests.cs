using DSAExperimentation.DataStructures.Graph.ShortestPaths;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TopKFrequentElements;

// LeetCode 347. Top K Frequent Elements: a HashMap<int,int> counts each value's
// occurrences, then a size-k min-heap keeps only the k most frequent values -
// this repo's own Heap<T,TOrder> ordered by ByPriorityOrder<TNode,TWeight> (the
// same (node, priority) projection Dijkstra/A*'s frontier already uses in
// ShortestPath.cs), here projecting onto (value, frequency) instead of
// (node, distance). Discards its lowest-frequency root whenever the heap grows
// past k, the same size-k-heap shape KthLargestElementTests uses for LeetCode 215.
public sealed partial class TopKFrequentElementsTests
{
    [Fact]
    public void TopKFrequent_ClassicExample_ReturnsTheKMostFrequentValues()
    {
        int[] nums = [1, 1, 1, 2, 2, 3];

        var result = TopKFrequent(nums, k: 2);

        Assert.Equal(new HashSet<int> { 1, 2 }, new HashSet<int>(result));
    }

    [Fact]
    public void TopKFrequent_SingleElementRepeated_ReturnsThatElement()
    {
        int[] nums = [1];

        var result = TopKFrequent(nums, k: 1);

        Assert.Equal([1], result);
    }

    private static int[] TopKFrequent(int[] nums, int k)
    {
        var counts = new HashMap<int, int>();

        foreach (var value in nums)
        {
            counts.TryGetValue(value, out var count);
            counts.Set(value, count + 1);
        }

        var heap = new Heap<(int Node, int Priority), ByPriorityOrder<int, int>>();

        foreach (var value in counts.Keys)
        {
            counts.TryGetValue(value, out var frequency);
            heap.Push((value, frequency));

            if (heap.Count > k)
            {
                heap.TryPop(out _);
            }
        }

        var result = new int[heap.Count];

        for (var i = result.Length - 1; i >= 0; i--)
        {
            heap.TryPop(out var top);
            result[i] = top.Node;
        }

        return result;
    }
}
