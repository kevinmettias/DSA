using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindKthLargestXorCoordinateValue;

// LeetCode 1738. Find Kth Largest XOR Coordinate Value: a 2D prefix-XOR pass
// over the grid (the XOR analogue of a prefix-sum rectangle) feeds every
// coordinate value into the same O(rows*cols*log k) size-k min-heap
// KthLargestElementTests already uses - this repo's Heap<int,MinHeapOrder<int>>
// discarding its smallest root whenever the heap grows past k.
public sealed partial class FindKthLargestXorCoordinateValueTests
{
    [Theory]
    [InlineData(1, 7)]
    [InlineData(2, 5)]
    [InlineData(3, 4)]
    public void KthLargestValue_LeetCodeExample_ReturnsCorrectRank(int k, int expected)
    {
        int[][] matrix = [[5, 2], [1, 6]];

        var result = KthLargestValue(matrix, k);

        Assert.Equal(expected, result);
    }

    private static int KthLargestValue(int[][] matrix, int k)
    {
        var rows = matrix.Length;
        var cols = matrix[0].Length;
        var prefixXor = new int[rows + 1, cols + 1];
        var heap = new Heap<int, MinHeapOrder<int>>();

        for (var r = 1; r <= rows; r++)
        {
            for (var c = 1; c <= cols; c++)
            {
                prefixXor[r, c] = matrix[r - 1][c - 1] ^ prefixXor[r - 1, c] ^ prefixXor[r, c - 1] ^ prefixXor[r - 1, c - 1];

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
}
