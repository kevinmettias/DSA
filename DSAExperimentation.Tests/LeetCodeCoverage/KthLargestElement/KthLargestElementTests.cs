using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.KthLargestElement;

// LeetCode 215. Kth Largest Element in an Array: an O(n log k) size-k min-heap,
// this repo's Heap<T,MinHeapOrder<T>> discarding its smallest root whenever the
// heap grows past k.
public sealed partial class KthLargestElementTests
{
    [Fact]
    public void FindKthLargest_ClassicExample_ReturnsCorrectRank()
    {
        int[] nums = [3, 2, 1, 5, 6, 4];

        var result = FindKthLargest(nums, rank: 2);

        Assert.Equal(5, result);
    }

    private static int FindKthLargest(int[] nums, int rank)
    {
        var heap = new Heap<int, MinHeapOrder<int>>();

        foreach (var value in nums)
        {
            heap.Push(value);

            if (heap.Count > rank)
            {
                heap.TryPop(out _);
            }
        }

        heap.TryPeek(out var kthLargest);
        return kthLargest;
    }
}
