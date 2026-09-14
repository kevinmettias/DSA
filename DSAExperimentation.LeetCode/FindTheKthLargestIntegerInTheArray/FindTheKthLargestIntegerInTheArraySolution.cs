using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.FindTheKthLargestIntegerInTheArray;

// LeetCode 1985. Find the Kth Largest Integer in the Array: nums holds arbitrarily
// large non-negative integers as digit strings, too big for long, so "largest"
// cannot use string's own IComparable<string> - lexicographic order is wrong the
// moment lengths differ ("9" sorts after "10"). NumericStringToken below carries the
// correct numeric order instead: the longer digit string wins, and equal lengths
// fall back to an ordinal digit-by-digit compare. It is meaningless outside this
// problem's own input encoding, so it stays here rather than in Algorithms/ or
// Domain/.
//
// The two strategies differ in how much of the array they actually order: a full
// O(n log n) sort followed by a direct index, or an O(n log k) size-k min-heap
// (this repo's own Heap<T, MinHeapOrder<T>>) that discards its smallest root
// whenever it grows past k - the exact shape KthLargestElementSolution establishes
// for LC 215, just over this comparable wrapper instead of int.
internal static class FindTheKthLargestIntegerInTheArraySolution
{
    // The textbook answer: sort a copy under the numeric comparison and index from
    // the end. Deliberately written without this repo's primitives - it is the arm
    // the size-k heap has to justify itself against.
    public static string KthLargestNumberByFullSort(string[] nums, int rank)
    {
        var copy = (string[])nums.Clone();
        Array.Sort(copy, CompareNumeric);
        return copy[^rank];
    }

    // The size-k min-heap: push every value and evict the root as soon as the heap
    // grows past rank, so the heap only ever holds the rank largest values seen and
    // its root is the answer. The log factor is on rank, not on the array length.
    public static string KthLargestNumberBySizeKMinHeap(string[] nums, int rank)
    {
        var heap = new Heap<NumericStringToken, MinHeapOrder<NumericStringToken>>();

        foreach (var value in nums)
        {
            heap.Push(new NumericStringToken(value));

            if (heap.Count > rank)
            {
                heap.TryPop(out _);
            }
        }

        heap.TryPeek(out var kthLargest);
        return kthLargest.Value;
    }

    // Numeric order over digit strings: a longer string is a larger number (LC 1985
    // guarantees no leading zeros), and equal lengths compare digit by digit, which
    // ordinal string comparison already does.
    private static int CompareNumeric(string first, string second) => first.Length != second.Length
        ? first.Length.CompareTo(second.Length)
        : string.CompareOrdinal(first, second);

    private readonly record struct NumericStringToken(string Value) : IComparable<NumericStringToken>
    {
        public int CompareTo(NumericStringToken other) => CompareNumeric(Value, other.Value);
    }
}
