using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.SpiralMatrixIV;

// LeetCode 2326. Spiral Matrix IV: lay a singly linked list's values into a
// rowCount x columnCount matrix along the clockwise spiral path, leaving every cell
// the list never reaches at -1.
//
// Both strategies walk this repo's own SinglyLinkedListNode<int> exactly once
// (AddTwoNumbersSolution's precedent for the primitive); what differs is how the
// spiral path itself is decided. DirectionArray is the common direction-vector
// approach - turn whenever the next cell is out of bounds or already filled -
// which needs a second rowCount x columnCount bool matrix purely to answer "have I
// been here" and a bounds/visited check on every single cell. BoundaryShrink instead
// pulls one of four cursors in after each side is walked, so a turn is decided once
// per side rather than once per cell, with no allocation beyond the answer itself.
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
    public static int[][] SpiralMatrixByDirectionArray(int rowCount, int columnCount, SinglyLinkedListNode<int>? head)
    {
        var matrix = EmptyMatrix(rowCount, columnCount);
        var visited = new bool[rowCount, columnCount];
        (int DeltaRow, int DeltaColumn)[] directions = [(0, 1), (1, 0), (0, -1), (-1, 0)];
        int row = 0, column = 0, direction = 0;

        for (var node = head; node is not null; node = node.Next)
        {
            matrix[row][column] = node.Value;
            visited[row, column] = true;
            (row, column, direction) =
                NextSpiralStep((row, column, direction), (visited, directions, rowCount, columnCount));
        }

        return matrix;
    }

    // Where the cursor goes after writing one cell: one step along the current
    // direction, turning right first when that step would leave the grid or land on
    // a cell already written.
    private static (int Row, int Column, int Direction) NextSpiralStep(
        (int Row, int Column, int Direction) cursor,
        (bool[,] Visited, (int DeltaRow, int DeltaColumn)[] Directions, int Rows, int Columns) grid)
    {
        var (row, column, direction) = cursor;
        var (deltaRow, deltaColumn) = grid.Directions[direction];
        var nextRow = row + deltaRow;
        var nextColumn = column + deltaColumn;

        if (IsOutsideGrid(nextRow, nextColumn, grid.Rows, grid.Columns) || grid.Visited[nextRow, nextColumn])
        {
            direction = (direction + 1) % grid.Directions.Length;
            (deltaRow, deltaColumn) = grid.Directions[direction];
            nextRow = row + deltaRow;
            nextColumn = column + deltaColumn;
        }

        return (nextRow, nextColumn, direction);
    }

    // Both coordinates have to land inside the rowCount x columnCount matrix.
    private static bool IsOutsideGrid(int row, int column, int rows, int columns) =>
        row < 0 || row >= rows || column < 0 || column >= columns;

    // Four cursors bound the still-unwritten rectangle; each side is walked in
    // full and then its cursor is pulled in, so the walk never asks whether a
    // cell was already written. Running out of list simply stops every loop
    // where it stands, leaving the rest of the matrix at EmptyCell.
    public static int[][] SpiralMatrixByBoundaryShrink(int rowCount, int columnCount, SinglyLinkedListNode<int>? head)
    {
        var matrix = EmptyMatrix(rowCount, columnCount);
        var node = head;
        var bounds = (Top: 0, Bottom: rowCount - 1, Left: 0, Right: columnCount - 1);

        while (HasUnwrittenCells(bounds.Top, bounds.Bottom, bounds.Left, bounds.Right) && node is not null)
        {
            (bounds, node) = ShrinkOneRing(matrix, node, bounds);
        }

        return matrix;
    }

    // The shrunken rectangle still has cells to write while both of its pairs of cursors
    // have not crossed.
    private static bool HasUnwrittenCells(int top, int bottom, int left, int right) =>
        top <= bottom && left <= right;

    // Walk the current rectangle's four sides in order, pulling each cursor in after
    // the side it bounds, and hand back the ring that is left along with where the
    // list got to.
    private static ((int Top, int Bottom, int Left, int Right) Bounds, SinglyLinkedListNode<int>? Node) ShrinkOneRing(
        int[][] matrix, SinglyLinkedListNode<int>? node, (int Top, int Bottom, int Left, int Right) bounds)
    {
        var (top, bottom, left, right) = bounds;

        node = WriteRun(matrix, node, (top, left, 0, 1), (right - left) + 1);
        top++;

        node = WriteRun(matrix, node, (top, right, 1, 0), (bottom - top) + 1);
        right--;

        if (top <= bottom)
        {
            node = WriteRun(matrix, node, (bottom, right, 0, -1), (right - left) + 1);
            bottom--;
        }

        if (left <= right)
        {
            node = WriteRun(matrix, node, (bottom, left, -1, 0), (bottom - top) + 1);
            left++;
        }

        return ((top, bottom, left, right), node);
    }

    // Copy the list into one straight run of the matrix, a cell per step, stopping
    // early when the list runs out; hand back where the list got to.
    private static SinglyLinkedListNode<int>? WriteRun(
        int[][] matrix,
        SinglyLinkedListNode<int>? node,
        (int Row, int Column, int RowStep, int ColumnStep) run,
        int length)
    {
        for (var step = 0; step < length && node is not null; step++)
        {
            matrix[run.Row][run.Column] = node.Value;
            node = node.Next;
            run.Row += run.RowStep;
            run.Column += run.ColumnStep;
        }

        return node;
    }

    // Both strategies start from the same all-empty matrix, so the fill lives
    // here rather than being written out twice and drifting.
    private static int[][] EmptyMatrix(int rowCount, int columnCount)
    {
        var matrix = new int[rowCount][];

        for (var row = 0; row < rowCount; row++)
        {
            matrix[row] = new int[columnCount];
            Array.Fill(matrix[row], EmptyCell);
        }

        return matrix;
    }
}
