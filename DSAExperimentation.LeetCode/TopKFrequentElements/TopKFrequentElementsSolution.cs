using DSAExperimentation.DataStructures.Graph.ShortestPaths;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.TopKFrequentElements;

// LeetCode 347. Top K Frequent Elements: the topCount values that occur most
// often in nums, in any order.
//
// The baseline counts with the BCL's own Dictionary and sorts every distinct
// value by frequency (O(d log d) over d distinct values). The composed strategy
// counts with this repo's own HashMap<int, int>, then keeps only the topCount
// most frequent in a size-k min-heap ordered by ByPriorityOrder<TNode, TWeight>
// (the same (node, priority) projection Dijkstra/A*'s frontier already uses in
// ShortestPath.cs), here projecting onto (value, frequency) instead of
// (node, distance) - O(d log k), discarding its lowest-frequency root whenever
// the heap grows past topCount, the same size-k-heap shape
// KthLargestElementSolution uses for LeetCode 215.
internal static class TopKFrequentElementsSolution
{
    // The textbook approach: count with a BCL Dictionary, then sort every
    // distinct value by frequency and take the first topCount. Deliberately
    // written without this repo's primitives - the arm the size-k heap strategy
    // below has to justify itself against.
    public static int[] FindTopKFrequentByFullSort(int[] nums, int topCount)
    {
        var counts = new Dictionary<int, int>();

        foreach (var value in nums)
        {
            counts[value] = counts.GetValueOrDefault(value) + 1;
        }

        return counts
            .OrderByDescending(entry => entry.Value)
            .Take(topCount)
            .Select(entry => entry.Key)
            .ToArray();
    }

    public static int[] FindTopKFrequentBySizeKMinHeap(int[] nums, int topCount)
    {
        var counts = CountFrequencies(nums);
        var heap = BuildSizeKMinHeap(counts, topCount);

        return DrainHeapDescending(heap);
    }

    private static HashMap<int, int> CountFrequencies(int[] nums)
    {
        var counts = new HashMap<int, int>();

        foreach (var value in nums)
        {
            counts.TryGetValue(value, out var count);
            counts.Set(value, count + 1);
        }

        return counts;
    }

    private static Heap<(int Node, int Priority), ByPriorityOrder<int, int>> BuildSizeKMinHeap(
        HashMap<int, int> counts, int topCount)
    {
        var heap = new Heap<(int Node, int Priority), ByPriorityOrder<int, int>>();

        foreach (var value in counts.Keys)
        {
            counts.TryGetValue(value, out var frequency);
            heap.Push((value, frequency));

            if (heap.Count > topCount)
            {
                heap.TryPop(out _);
            }
        }

        return heap;
    }

    private static int[] DrainHeapDescending(Heap<(int Node, int Priority), ByPriorityOrder<int, int>> heap)
    {
        var result = new int[heap.Count];

        for (var i = result.Length - 1; i >= 0; i--)
        {
            heap.TryPop(out var top);
            result[i] = top.Node;
        }

        return result;
    }
}
