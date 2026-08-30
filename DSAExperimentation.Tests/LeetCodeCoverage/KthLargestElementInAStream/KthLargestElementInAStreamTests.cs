using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.KthLargestElementInAStream;

// LeetCode 703. Kth Largest Element in a Stream: a size-k min-heap using this repo's
// own Heap<T,MinHeapOrder<T>> - the same "discard the smallest root once the heap
// grows past k" approach KthLargestElementTests already uses for LC215's one-shot
// array, wired up as a stateful class here since LC703 asks for the running
// kth-largest across an open-ended Add stream instead of a single array.
public sealed partial class KthLargestElementInAStreamTests
{
    [Fact]
    public void Add_LeetCodeExampleSequence_ReturnsRunningKthLargest()
    {
        var kthLargest = new KthLargestStream(3, [4, 5, 8, 2]);

        Assert.Equal(4, kthLargest.Add(3));
        Assert.Equal(5, kthLargest.Add(5));
        Assert.Equal(5, kthLargest.Add(10));
        Assert.Equal(8, kthLargest.Add(9));
        Assert.Equal(8, kthLargest.Add(4));
    }

    private sealed class KthLargestStream
    {
        private readonly int _k;
        private readonly Heap<int, MinHeapOrder<int>> _heap = new();

        public KthLargestStream(int k, int[] nums)
        {
            _k = k;

            foreach (var num in nums)
            {
                Add(num);
            }
        }

        public int Add(int val)
        {
            _heap.Push(val);

            if (_heap.Count > _k)
            {
                _heap.TryPop(out _);
            }

            _heap.TryPeek(out var kthLargest);
            return kthLargest;
        }
    }
}
