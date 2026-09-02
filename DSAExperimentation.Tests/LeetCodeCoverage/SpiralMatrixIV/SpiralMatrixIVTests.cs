using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SpiralMatrixIV;

// LeetCode 2326. Spiral Matrix IV: walks this repo's own SinglyLinkedListNode<int>
// once (AddTwoNumbersTests' precedent for the primitive) while filling an m x n
// matrix along the classic boundary-shrinking spiral path (top/bottom/left/right
// each pulled in one step after its side is walked). Every cell starts at -1, so
// once the list is exhausted the walk simply stops early and the remaining cells
// stay unfilled, exactly as LeetCode specifies.
public sealed partial class SpiralMatrixIVTests
{
    [Fact]
    public void SpiralMatrix_ClassicExample_FillsClockwiseThenPadsRemainderWithNegativeOne()
    {
        var head = BuildList([3, 0, 2, 6, 8, 1, 7, 9, 4, 2, 5, 5, 3, 9]);

        var matrix = SpiralMatrix(3, 5, head);

        int[][] expected =
        [
            [3, 0, 2, 6, 8],
            [5, 3, 9, -1, 1],
            [5, 2, 4, 9, 7],
        ];
        Assert.Equal(expected, matrix);
    }

    [Fact]
    public void SpiralMatrix_SingleRow_FillsLeftToRightThenPads()
    {
        var head = BuildList([0, 1, 2]);

        var matrix = SpiralMatrix(1, 4, head);

        int[][] expected = [[0, 1, 2, -1]];
        Assert.Equal(expected, matrix);
    }

    [Fact]
    public void SpiralMatrix_ShorterListThanGrid_LeavesRemainingCellsNegativeOne()
    {
        var head = BuildList([7]);

        var matrix = SpiralMatrix(2, 2, head);

        int[][] expected =
        [
            [7, -1],
            [-1, -1],
        ];
        Assert.Equal(expected, matrix);
    }

    private static int[][] SpiralMatrix(int m, int n, SinglyLinkedListNode<int>? head)
    {
        var matrix = new int[m][];
        for (var row = 0; row < m; row++)
        {
            matrix[row] = new int[n];
            Array.Fill(matrix[row], -1);
        }

        var node = head;
        int top = 0, bottom = m - 1, left = 0, right = n - 1;

        while (top <= bottom && left <= right && node is not null)
        {
            for (var col = left; col <= right && node is not null; col++)
            {
                matrix[top][col] = node.Value;
                node = node.Next;
            }

            top++;

            for (var row = top; row <= bottom && node is not null; row++)
            {
                matrix[row][right] = node.Value;
                node = node.Next;
            }

            right--;

            if (top <= bottom)
            {
                for (var col = right; col >= left && node is not null; col--)
                {
                    matrix[bottom][col] = node.Value;
                    node = node.Next;
                }

                bottom--;
            }

            if (left <= right)
            {
                for (var row = bottom; row >= top && node is not null; row--)
                {
                    matrix[row][left] = node.Value;
                    node = node.Next;
                }

                left++;
            }
        }

        return matrix;
    }

    private static SinglyLinkedListNode<int>? BuildList(int[] values)
    {
        SinglyLinkedListNode<int>? head = null;
        SinglyLinkedListNode<int>? tail = null;

        foreach (var value in values)
        {
            var node = new SinglyLinkedListNode<int>(value);
            head ??= node;
            AppendAfter(tail, node);
            tail = node;
        }

        return head;
    }

    // No previous node to link on the very first iteration (tail is still null) -
    // head itself becomes that first node instead, back in BuildList.
    private static void AppendAfter(SinglyLinkedListNode<int>? tail, SinglyLinkedListNode<int> node)
    {
        if (tail is not null)
        {
            tail.Next = node;
        }
    }
}
