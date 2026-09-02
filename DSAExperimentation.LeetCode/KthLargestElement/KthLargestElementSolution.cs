using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.KthLargestElement;

// LeetCode 215. Kth Largest Element in an Array: the k-th largest value, 1-indexed
// from the top (rank 1 is the maximum).
//
// The two strategies differ in how much of the array they actually order: a full
// O(n log n) sort followed by a direct index, or an O(n log k) size-k min-heap
// (this repo's own Heap<T, MinHeapOrder<T>>) that discards its smallest root
// whenever it grows past k, so its own log factor is on k rather than n.
internal static class KthLargestElementSolution
{
    // The textbook answer: sort a copy and index from the end. Deliberately
    // written without this repo's primitives - it is the arm the size-k heap has
    // to justify itself against.
    public static int FindKthLargestByFullSort(int[] nums, int rank)
    {
        var copy = (int[])nums.Clone();
        Array.Sort(copy);
        return copy[^rank];
    }

    public static int FindKthLargestBySizeKMinHeap(int[] nums, int rank)
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
