using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.LeetCode.FindTriangularSumOfAnArray;

// LeetCode 2221. Find Triangular Sum of an Array: repeatedly replace the row with
// the pairwise sums of its adjacent elements, each taken mod 10, until one element
// is left - that element is the answer.
//
// No closed-form shortcut (a Lucas'-theorem walk over Pascal's triangle mod 10) is
// composable from this repo's primitives - nothing here provides modular-binomial
// machinery - so both strategies run the identical O(n^2) pairwise reduction and
// differ only in the container the row lives in: a raw int[] rewritten in place, or
// this repo's own DynamicArray<int> rebuilt one row at a time.
internal static class FindTriangularSumOfAnArraySolution
{
    // Each element of every row is a single decimal digit, so every pairwise sum is
    // reduced mod 10.
    private const int DigitModulus = 10;

    // The textbook answer: one int[] buffer, each round overwriting the first
    // size - 1 slots in place and shrinking the live prefix by one. Deliberately
    // BCL-only - it is the arm the DynamicArray strategy below is measured against.
    // The caller's array is cloned rather than mutated.
    public static int TriangularSumByInPlaceArray(int[] nums)
    {
        var current = (int[])nums.Clone();
        var size = current.Length;

        while (size > 1)
        {
            for (var i = 0; i < size - 1; i++)
            {
                current[i] = (current[i] + current[i + 1]) % DigitModulus;
            }

            size--;
        }

        return current[0];
    }

    // This repo's own DynamicArray<int>: Add to seed the first row, then Count/Get
    // to fold each row into a fresh shorter one. Nothing else about the reduction
    // changes, so what the comparison isolates is the primitive's own overhead.
    public static int TriangularSumByDynamicArray(int[] nums)
    {
        var current = BuildRow(nums);

        while (current.Count > 1)
        {
            current = ReduceOnce(current);
        }

        return current.Get(0);
    }

    private static DynamicArray<int> BuildRow(int[] nums)
    {
        var row = new DynamicArray<int>();

        foreach (var num in nums)
        {
            row.Add(num);
        }

        return row;
    }

    private static DynamicArray<int> ReduceOnce(DynamicArray<int> row)
    {
        var next = new DynamicArray<int>();

        for (var i = 0; i < row.Count - 1; i++)
        {
            next.Add((row.Get(i) + row.Get(i + 1)) % DigitModulus);
        }

        return next;
    }
}
