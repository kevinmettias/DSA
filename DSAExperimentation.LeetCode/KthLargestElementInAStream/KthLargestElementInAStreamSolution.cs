using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.KthLargestElementInAStream;

// LeetCode 703. Kth Largest Element in a Stream: a design problem - LeetCode's own
// shape is a stateful object seeded with k and an initial array, exposing a single
// Add operation that returns the running kth-largest value after each insert - the
// same design-problem shape FindMedianFromDataStreamSolution uses for LC 295.
//
// IKthLargestStream is bespoke to this problem alone - no other LeetCode entry
// shares an Add-returns-running-order-statistic contract - so it stays here rather
// than in DataStructures/.
internal static class KthLargestElementInAStreamSolution
{
    // The textbook baseline this composition has to justify itself against: a BCL
    // List<int>, re-sorted from scratch on every Add call. Deliberately written
    // without this repo's primitives.
    public static IKthLargestStream CreateBySortOnEveryAdd(int k, IEnumerable<int> nums) =>
        new SortOnEveryAddStream(k, nums);

    // The composed answer: this repo's own size-k Heap<int, MinHeapOrder<int>>,
    // discarding the smallest root whenever it grows past k - the same approach
    // KthLargestElementSolution.FindKthLargestBySizeKMinHeap uses for LC 215's
    // one-shot array, wired up as running state here since LC703 asks for the
    // running kth-largest across an open-ended Add stream instead of a single
    // array.
    public static IKthLargestStream CreateBySizeKMinHeap(int k, IEnumerable<int> nums) =>
        new SizeKMinHeapStream(k, nums);

    private sealed class SortOnEveryAddStream : IKthLargestStream
    {
        private readonly int _k;
        private readonly List<int> _values = [];

        public SortOnEveryAddStream(int k, IEnumerable<int> nums)
        {
            _k = k;

            foreach (var num in nums)
            {
                Add(num);
            }
        }

        public int Add(int val)
        {
            _values.Add(val);

            var sorted = _values.ToArray();
            Array.Sort(sorted);
            return sorted[Math.Max(0, sorted.Length - _k)];
        }
    }

    private sealed class SizeKMinHeapStream : IKthLargestStream
    {
        private readonly int _k;
        private readonly Heap<int, MinHeapOrder<int>> _heap = new();

        public SizeKMinHeapStream(int k, IEnumerable<int> nums)
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

// The Add contract every strategy above implements. Bespoke to this problem: no
// other LeetCode entry shares this shape, so it stays here rather than in
// DataStructures/.
internal interface IKthLargestStream
{
    int Add(int val);
}
