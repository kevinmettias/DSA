using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.SpiralMatrixIV;

// LeetCode 2326. Spiral Matrix IV: lay a singly linked list's values into an
// m x n matrix along the clockwise spiral path, leaving every cell the list never
// reaches at -1.
//
// Both strategies walk this repo's own SinglyLinkedListNode<int> exactly once
// (AddTwoNumbersSolution's precedent for the primitive); what differs is how the
// spiral path itself is decided. DirectionArray is the common direction-vector
// approach - turn whenever the next cell is out of bounds or already filled -
// which needs a second m x n bool matrix purely to answer "have I been here" and
// a bounds/visited check on every single cell. BoundaryShrink instead pulls one
// of four cursors in after each side is walked, so a turn is decided once per
// side rather than once per cell, with no allocation beyond the answer itself.
internal static class SpiralMatrixIVSolution
{
    // LeetCode's filler for a cell the list runs out before reaching. Not
    // LeetCodeAnswer.None: that is "there is no valid answer", while this is a
    // real answer with an unfilled cell in it.
    private const int EmptyCell = -1;

    // The textbook answer: a direction vector plus a visited matrix, turning
    // right whenever the next step would leave the grid or revisit a cell.
    // Deliberately written with nothing but BCL arrays - it is the arm
    // BoundaryShrink has to justify itself against.
    public static int[][] SpiralMatrixByDirectionArray(int m, int n, SinglyLinkedListNode<int>? head)
    {
        var matrix = EmptyMatrix(m, n);
        var visited = new bool[m, n];
        (int DeltaRow, int DeltaColumn)[] directions = [(0, 1), (1, 0), (0, -1), (-1, 0)];
        int row = 0, column = 0, direction = 0;

        for (var node = head; node is not null; node = node.Next)
        {
            matrix[row][column] = node.Value;
            visited[row, column] = true;

            var (deltaRow, deltaColumn) = directions[direction];
            var nextRow = row + deltaRow;
            var nextColumn = column + deltaColumn;

            if (nextRow < 0 || nextRow >= m || nextColumn < 0 || nextColumn >= n || visited[nextRow, nextColumn])
            {
                direction = (direction + 1) % directions.Length;
                (deltaRow, deltaColumn) = directions[direction];
                nextRow = row + deltaRow;
                nextColumn = column + deltaColumn;
            }

            row = nextRow;
            column = nextColumn;
        }

        return matrix;
    }

    // Four cursors bound the still-unwritten rectangle; each side is walked in
    // full and then its cursor is pulled in, so the walk never asks whether a
    // cell was already written. Running out of list simply stops every loop
    // where it stands, leaving the rest of the matrix at EmptyCell.
    public static int[][] SpiralMatrixByBoundaryShrink(int m, int n, SinglyLinkedListNode<int>? head)
    {
        var matrix = EmptyMatrix(m, n);
        var node = head;
        int top = 0, bottom = m - 1, left = 0, right = n - 1;

        while (top <= bottom && left <= right && node is not null)
        {
            for (var column = left; column <= right && node is not null; column++)
            {
                matrix[top][column] = node.Value;
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
                for (var column = right; column >= left && node is not null; column--)
                {
                    matrix[bottom][column] = node.Value;
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

    // Both strategies start from the same all-empty matrix, so the fill lives
    // here rather than being written out twice and drifting.
    private static int[][] EmptyMatrix(int m, int n)
    {
        var matrix = new int[m][];

        for (var row = 0; row < m; row++)
        {
            matrix[row] = new int[n];
            Array.Fill(matrix[row], EmptyCell);
        }

        return matrix;
    }
}
