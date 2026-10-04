using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.RankTransformOfAMatrix;

// LeetCode 1632. Rank Transform of a Matrix: replace every cell with the
// smallest rank consistent with three rules - ranks start at 1, a strictly
// smaller value in the same row or column gets a strictly smaller rank, and two
// equal values sharing a row or column get the same rank (transitively).
//
// MatrixRankTransformByDisjointSetRanking is the single-pass answer: sort every
// cell by value with this repo's MergeSort over an ArrayIndexedSequence (the
// same convention AccountsMergeSolution and QueueReconstructionByHeightSolution
// use), then handle one equal-value batch at a time. One DisjointSet over the
// cells' sorted positions, allocated once, joins each cell to the batch's
// previous cell in its row and in its column. Two equal cells only share a rank
// when a chain of shared rows/columns actually connects them, which is exactly
// what the union-find partition captures; each component's rank is 1 + the
// largest rank already assigned to any row or column it touches. Nothing is
// allocated per batch: a DisjointSet over every row and column for each batch
// cost O(rows + cols) per distinct value, and on LeetCode's 500 x 500 a matrix
// of distinct values has 250,000 of them.
//
// MatrixRankTransformByIterativeRelaxation is the textbook alternative: start
// every rank at 1 and rescan every row and column, nudging a rank up whenever a
// pair violates the ordering or tie rule, until a full pass changes nothing - a
// Bellman-Ford-shaped least fixed point over the rank constraints.
internal static class RankTransformOfAMatrixSolution
{
    private const int LowestRank = 1;

    // Deliberately written without this repo's primitives - plain jagged int
    // arrays relaxed until stable is what you would write without this repo.
    public static int[][] MatrixRankTransformByIterativeRelaxation(int[][] matrix)
    {
        var rows = matrix.Length;
        var cols = matrix[0].Length;
        var rank = CreateInitialRanks(rows, cols);

        RelaxRanksUntilStable(matrix, rows, cols, rank);

        return rank;
    }

    private static int[][] CreateInitialRanks(int rows, int cols)
    {
        var rank = new int[rows][];

        for (var r = 0; r < rows; r++)
        {
            rank[r] = new int[cols];
            Array.Fill(rank[r], LowestRank);
        }

        return rank;
    }

    private static void RelaxRanksUntilStable(int[][] matrix, int rows, int cols, int[][] rank)
    {
        var changed = true;

        while (changed)
        {
            changed = false;
            changed |= TryRelaxRows(matrix, rows, cols, rank);
            changed |= TryRelaxColumns(matrix, rows, cols, rank);
        }
    }

    private static bool TryRelaxRows(int[][] matrix, int rows, int cols, int[][] rank)
    {
        var changed = false;

        for (var r = 0; r < rows; r++)
        {
            changed |= TryRelaxLine(cols, new MatrixRow(matrix, rank[r], r));
        }

        return changed;
    }

    private static bool TryRelaxColumns(int[][] matrix, int rows, int cols, int[][] rank)
    {
        var changed = false;

        for (var c = 0; c < cols; c++)
        {
            changed |= TryRelaxLine(rows, new MatrixColumn(matrix, rank, c));
        }

        return changed;
    }

    // Sort once, then settle each equal-value batch as a unit: within a batch a
    // DisjointSet decides which cells are forced to share a rank, and the ranks
    // already assigned to the rows and columns that batch touches decide what
    // that shared rank is.
    public static int[][] MatrixRankTransformByDisjointSetRanking(int[][] matrix)
    {
        var rows = matrix.Length;
        var cols = matrix[0].Length;
        var cells = BuildCells(matrix, rows, cols);

        SortCellsByValue(cells);

        var result = CreateEmptyResult(rows, cols);
        var grid = new RankingGrid(cells, new EqualValueComponents(rows, cols), result, new int[rows], new int[cols]);
        var index = 0;

        while (index < cells.Length)
        {
            index = AdvancePastRankBatch(grid, index);
        }

        return result;
    }

    private static (int Value, int Row, int Col)[] BuildCells(int[][] matrix, int rows, int cols)
    {
        var cells = new (int Value, int Row, int Col)[rows * cols];

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                cells[(r * cols) + c] = (matrix[r][c], r, c);
            }
        }

        return cells;
    }

    private static void SortCellsByValue((int Value, int Row, int Col)[] cells)
    {
        var byValue = Comparer<(int Value, int Row, int Col)>.Create((a, b) => a.Value.CompareTo(b.Value));
        MergeSort.Sort(cells, byValue);
    }

    private static int[][] CreateEmptyResult(int rows, int cols)
    {
        var result = new int[rows][];

        for (var r = 0; r < rows; r++)
        {
            result[r] = new int[cols];
        }

        return result;
    }

    // Advances past the batch of equal-value cells starting at `index`, assigning
    // them ranks as one group, and returns the index where the next batch starts.
    private static int AdvancePastRankBatch(RankingGrid grid, int index)
    {
        var end = index;

        while (end < grid.Cells.Length && grid.Cells[end].Value == grid.Cells[index].Value)
        {
            end++;
        }

        AssignBatchRanks(grid, index, end);
        return end;
    }

    // Three passes over [start, end): join the batch into its components, raise each
    // component's best to the highest rank its rows and columns already hold, then
    // write every cell one past its component's best.
    private static void AssignBatchRanks(RankingGrid grid, int start, int end)
    {
        for (var position = start; position < end; position++)
        {
            grid.Ties.Join(position, grid.Cells[position].Row, grid.Cells[position].Col, start);
        }

        for (var position = start; position < end; position++)
        {
            RecordBestCandidate(grid, position);
        }

        for (var position = start; position < end; position++)
        {
            AssignRankToCell(grid, position);
        }
    }

    private static void RecordBestCandidate(RankingGrid grid, int position)
    {
        var (_, row, col) = grid.Cells[position];
        var candidate = Math.Max(grid.RowRank[row], grid.ColRank[col]);

        grid.Ties.RaiseBest(position, candidate);
    }

    private static void AssignRankToCell(RankingGrid grid, int position)
    {
        var (_, row, col) = grid.Cells[position];

        WriteCellRank(grid, row, col, grid.Ties.BestOf(position) + 1);
    }

    // Writes one cell's final rank through to every place the grid stores it, so a
    // later cell in the same component reads the raised rank back out.
    private static void WriteCellRank(RankingGrid grid, int row, int col, int rank)
    {
        grid.Result[row][col] = rank;
        grid.RowRank[row] = rank;
        grid.ColRank[col] = rank;
    }

    // One row or one column of the rank grid, read the two ways TryRelaxLine needs it:
    // the matrix value at an index, and the rank cell at that index. Which line is
    // being relaxed is fixed for the whole pass, so the row-or-column choice is the
    // only thing an implementation has to know.
    private interface IRankLine
    {
        int ValueAt(int index);

        RankCell CellAt(int index);
    }

    private static bool TryRelaxLine(int count, IRankLine line)
    {
        var changed = false;

        for (var i = 0; i < count; i++)
        {
            for (var j = i + 1; j < count; j++)
            {
                changed |= TryRelaxPair(line.ValueAt(i), line.ValueAt(j), line.CellAt(i), line.CellAt(j));
            }
        }

        return changed;
    }

    private static bool TryRelaxPair(int valueA, int valueB, RankCell cellA, RankCell cellB)
    {
        if (valueA < valueB && cellA.Rank >= cellB.Rank)
        {
            cellB.Rank = cellA.Rank + 1;
            return true;
        }

        if (valueB < valueA && cellB.Rank >= cellA.Rank)
        {
            cellA.Rank = cellB.Rank + 1;
            return true;
        }

        if (valueA == valueB && cellA.Rank != cellB.Rank)
        {
            var merged = Math.Max(cellA.Rank, cellB.Rank);
            cellA.Rank = merged;
            cellB.Rank = merged;
            return true;
        }

        return false;
    }

    // A single relaxable cell, held as the row it lives in plus its column so
    // that writing through it updates the shared rank grid in place.
    private readonly record struct RankCell(int[] Row, int Col)
    {
        public int Rank
        {
            get => Row[Col];
            set => Row[Col] = value;
        }
    }

    private readonly record struct RankingGrid(
        (int Value, int Row, int Col)[] Cells, EqualValueComponents Ties, int[][] Result, int[] RowRank, int[] ColRank);

    // The tie components of every batch, over one DisjointSet of positions in the
    // sorted cell order. A batch only ever unions its own positions - each with the
    // batch's previous position in the same row and in the same column - so no
    // component reaches across batches, and nothing is cleared or reallocated between
    // them. A line's last position belongs to the current batch exactly when it is at
    // or past the batch's start, since every earlier batch sorts wholly before it.
    private sealed class EqualValueComponents
    {
        private const int NoPosition = -1;

        private readonly DisjointSet _components;
        private readonly int[] _bestByRoot;
        private readonly int[] _lastInRow;
        private readonly int[] _lastInCol;

        public EqualValueComponents(int rows, int cols)
        {
            _components = new DisjointSet(rows * cols);
            _bestByRoot = new int[rows * cols];
            _lastInRow = CreateEmptyLines(rows);
            _lastInCol = CreateEmptyLines(cols);
        }

        private static int[] CreateEmptyLines(int count)
        {
            var lines = new int[count];
            Array.Fill(lines, NoPosition);

            return lines;
        }

        // Every position of a batch is joined before any is raised, so clearing its own
        // best here clears every root the batch can have.
        public void Join(int position, int row, int col, int batchStart)
        {
            _bestByRoot[position] = 0;
            JoinWithLineMate(_lastInRow, row, position, batchStart);
            JoinWithLineMate(_lastInCol, col, position, batchStart);
        }

        private void JoinWithLineMate(int[] lastInLine, int line, int position, int batchStart)
        {
            var mate = lastInLine[line];

            if (mate >= batchStart)
            {
                _components.Union(mate, position);
            }

            lastInLine[line] = position;
        }

        public void RaiseBest(int position, int candidate)
        {
            var root = _components.Find(position);
            _bestByRoot[root] = Math.Max(_bestByRoot[root], candidate);
        }

        public int BestOf(int position) => _bestByRoot[_components.Find(position)];
    }

    // A row of the matrix as a relaxable line: the cell at index (row, index) reads its
    // value straight out of the matrix and its rank through the row's own rank array.
    private sealed class MatrixRow(int[][] matrix, int[] rank, int row) : IRankLine
    {
        public int ValueAt(int index) => matrix[row][index];

        public RankCell CellAt(int index) => new(rank, index);
    }

    // The same read down a column: the value comes from (index, column) and the rank
    // through the rank array of the row that index names, which is what the rank grid
    // stores at that cell.
    private sealed class MatrixColumn(int[][] matrix, int[][] rank, int column) : IRankLine
    {
        public int ValueAt(int index) => matrix[index][column];

        public RankCell CellAt(int index) => new(rank[index], column);
    }
}
