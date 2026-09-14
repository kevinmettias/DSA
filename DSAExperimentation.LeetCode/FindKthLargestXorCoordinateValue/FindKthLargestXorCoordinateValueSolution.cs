using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.FindKthLargestXorCoordinateValue;

// LeetCode 1738. Find Kth Largest XOR Coordinate Value: the value of coordinate
// (a, b) is the XOR of every matrix[i][j] with i <= a and j <= b, and the answer is
// the kth largest of those rows*cols values.
//
// Both strategies share the same O(rows*cols) 2D prefix-XOR pass - the XOR analogue
// of a prefix-sum rectangle, where each cell folds in the cell above, the cell to
// the left, and back out the doubly-counted corner - and differ only in how they
// pick the kth largest value out of it afterwards: a full O(nm log nm) sort, or an
// O(nm log k) size-k min-heap whose log factor is on k rather than on the
// coordinate count.
internal static class FindKthLargestXorCoordinateValueSolution
{
    // The textbook answer: materialize every coordinate value, sort the lot, and
    // index k back from the end. Deliberately written over BCL arrays and
    // Array.Sort - it is the arm the heap below has to justify itself against.
    public static int KthLargestValueByFullSort(int[][] matrix, int k)
    {
        var values = PrefixXorValues(matrix);

        Array.Sort(values);

        return values[^k];
    }

    // This repo's own Heap<int, MinHeapOrder<int>>, capped at k: the root is the
    // smallest of the k largest values seen so far, so discarding it whenever the
    // heap outgrows k leaves the kth largest sitting at the root - exactly the
    // composition KthLargestElement uses for LC 215.
    public static int KthLargestValueBySizeKHeap(int[][] matrix, int k)
    {
        var prefixXor = PrefixXorTable(matrix);
        var heap = new Heap<int, MinHeapOrder<int>>();

        for (var r = 1; r < prefixXor.GetLength(0); r++)
        {
            for (var c = 1; c < prefixXor.GetLength(1); c++)
            {
                heap.Push(prefixXor[r, c]);

                if (heap.Count > k)
                {
                    heap.TryPop(out _);
                }
            }
        }

        heap.TryPeek(out var kthLargest);
        return kthLargest;
    }

    // A row/column of leading zeros lets every cell use the same recurrence with no
    // boundary special-casing, so the table is one larger than the matrix in each
    // dimension and coordinate (a, b) lands at [a + 1, b + 1].
    private static int[,] PrefixXorTable(int[][] matrix)
    {
        var rows = matrix.Length;
        var cols = matrix[0].Length;
        var prefixXor = new int[rows + 1, cols + 1];

        for (var r = 1; r <= rows; r++)
        {
            for (var c = 1; c <= cols; c++)
            {
                prefixXor[r, c] =
                    matrix[r - 1][c - 1] ^ prefixXor[r - 1, c] ^ prefixXor[r, c - 1] ^ prefixXor[r - 1, c - 1];
            }
        }

        return prefixXor;
    }

    private static int[] PrefixXorValues(int[][] matrix)
    {
        var prefixXor = PrefixXorTable(matrix);
        var values = new int[matrix.Length * matrix[0].Length];
        var index = 0;

        for (var r = 1; r < prefixXor.GetLength(0); r++)
        {
            for (var c = 1; c < prefixXor.GetLength(1); c++)
            {
                values[index++] = prefixXor[r, c];
            }
        }

        return values;
    }
}
