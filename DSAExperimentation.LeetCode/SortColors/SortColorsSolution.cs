using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.SortColors;

// LeetCode 75. Sort Colors: sort an array holding only 0s, 1s and 2s in place, in a
// single pass, without calling a general-purpose sort.
//
// The naive baseline is exactly that general-purpose sort - Array.Sort - the
// O(n log n) arm the linear Dutch-flag partition below has to justify itself
// against.
internal static class SortColorsSolution
{
    private const int HighColor = 2;

    // The textbook answer: hand the whole problem to Array.Sort. Deliberately
    // written without this repo's primitives.
    public static void SortByArraySort(int[] nums) => Array.Sort(nums);

    // Three-pointer Dutch national flag partition over this repo's own
    // ArrayIndexedSequence<int>: everything below `low` is already 0, everything
    // above `high` is already 2, and `mid` sweeps the unknown middle exactly once.
    public static void SortByDutchFlagPartition(int[] nums)
    {
        var seq = new ArrayIndexedSequence<int>(nums);
        var low = 0;
        var mid = 0;
        var high = seq.Length - 1;

        while (mid <= high)
        {
            if (seq.Get(mid) == 0)
            {
                Swap(seq, low++, mid++);
            }
            else if (seq.Get(mid) == HighColor)
            {
                Swap(seq, mid, high--);
            }
            else
            {
                mid++;
            }
        }
    }

    private static void Swap(ArrayIndexedSequence<int> seq, int first, int second)
    {
        var temp = seq.Get(first);
        seq.Set(first, seq.Get(second));
        seq.Set(second, temp);
    }
}
