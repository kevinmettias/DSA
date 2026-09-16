using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.RankTransformOfAMatrix;

// LeetCode 1632. Rank Transform of a Matrix: replace every cell with the
// smallest rank consistent with three rules - ranks start at 1, a strictly
// smaller value in the same row or column gets a strictly smaller rank, and two
// equal values sharing a row or column get the same rank (transitively).
//
// MatrixRankTransformByDisjointSetRanking is the single-pass answer: sort every
// cell by value with this repo's MergeSort over an ArrayIndexedSequence (the
// same convention AccountsMergeSolution and QueueReconstructionByHeightSolution
// use), then handle one equal-value batch at a time with a fresh DisjointSet
// over rows + cols nodes, unioning each cell's row node to its column node. Two
// equal cells only share a rank when a chain of shared rows/columns actually
// connects them, which is exactly what the union-find partition captures; each
// component's rank is 1 + the largest rank already assigned to any row or
// column it touches, tracked per-root in a HashMap<int,int>.
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
        var grid = new RankingGrid(cells, rows, result, new int[rows], new int[cols]);
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
        MergeSort.Sort<(int Value, int Row, int Col), ArrayIndexedSequence<(int Value, int Row, int Col)>>(
            new ArrayIndexedSequence<(int Value, int Row, int Col)>(cells), byValue);
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

    private static void AssignBatchRanks(RankingGrid grid, int start, int end)
    {
        var components = new DisjointSet(grid.Rows + grid.ColRank.Length);

        for (var i = start; i < end; i++)
        {
            components.Union(grid.Cells[i].Row, grid.Rows + grid.Cells[i].Col);
        }

        var bestByRoot = new HashMap<int, int>();

        for (var i = start; i < end; i++)
        {
            RecordBestCandidate(grid, grid.Cells[i], components, bestByRoot);
        }

        for (var i = start; i < end; i++)
        {
            AssignRankToCell(grid, grid.Cells[i], components, bestByRoot);
        }
    }

    private static void RecordBestCandidate(
        RankingGrid grid, (int Value, int Row, int Col) cell, DisjointSet components, HashMap<int, int> bestByRoot)
    {
        var (_, row, col) = cell;
        var root = components.Find(row);
        var candidate = Math.Max(grid.RowRank[row], grid.ColRank[col]);

        if (!bestByRoot.TryGetValue(root, out var best) || candidate > best)
        {
            bestByRoot.Set(root, candidate);
        }
    }

    private static void AssignRankToCell(
        RankingGrid grid, (int Value, int Row, int Col) cell, DisjointSet components, HashMap<int, int> bestByRoot)
    {
        var (_, row, col) = cell;
        var root = components.Find(row);
        bestByRoot.TryGetValue(root, out var best);

        WriteCellRank(grid, row, col, best + 1);
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
        (int Value, int Row, int Col)[] Cells, int Rows, int[][] Result, int[] RowRank, int[] ColRank);

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
