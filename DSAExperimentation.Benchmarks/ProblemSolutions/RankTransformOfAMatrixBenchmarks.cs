using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Rank Transform of a Matrix (LC 1632): IterativeRelaxation repeatedly
// rescans every row and column, nudging a cell's rank up whenever it
// violates a same-row/same-column ordering or tie constraint, until a full
// pass makes no change - a Bellman-Ford-shaped fixed point, same "relax
// until stable" idea as BellmanFordBenchmarks, just for a rank-ordering
// constraint instead of a distance. DisjointSetRanking is the single-pass
// approach RankTransformOfAMatrixTests proves out: MergeSort every cell by
// value, then union each equal-value batch's row/column nodes via this
// repo's own DisjointSet and assign ranks per connected component with a
// HashMap<int,int>, the same composition AccountsMergeTests already uses
// for a different problem. Values are random in [-10^5, 10^5), matching
// LeetCode's own constraint range.
[MemoryDiagnoser]
public class RankTransformOfAMatrixBenchmarks
{
    private const int RandomSeed = 1632; // LC problem number
    private const int ValueRange = 100_000;

    [Params(8, 20)]
    public int Size;

    private int[][] _matrix = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _matrix = new int[Size][];

        for (var r = 0; r < Size; r++)
        {
            _matrix[r] = new int[Size];

            for (var c = 0; c < Size; c++)
            {
                _matrix[r][c] = random.Next(-ValueRange, ValueRange);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int[][] IterativeRelaxation()
    {
        var rows = _matrix.Length;
        var cols = _matrix[0].Length;
        var rank = CreateInitialRanks(rows, cols);

        RelaxRanksUntilStable(rows, cols, rank);

        return rank;
    }

    private static int[][] CreateInitialRanks(int rows, int cols)
    {
        var rank = new int[rows][];

        for (var r = 0; r < rows; r++)
        {
            rank[r] = CreateInitialRankRow(cols);
        }

        return rank;
    }

    private static int[] CreateInitialRankRow(int cols)
    {
        var row = new int[cols];

        for (var c = 0; c < cols; c++)
        {
            row[c] = 1;
        }

        return row;
    }

    private void RelaxRanksUntilStable(int rows, int cols, int[][] rank)
    {
        var changed = true;

        while (changed)
        {
            changed = false;
            changed |= RelaxRows(rows, cols, rank);
            changed |= RelaxColumns(rows, cols, rank);
        }
    }

    private bool RelaxRows(int rows, int cols, int[][] rank)
    {
        var changed = false;

        for (var r = 0; r < rows; r++)
        {
            changed |= RelaxLine(cols, i => _matrix[r][i], i => new RankCell(rank[r], i));
        }

        return changed;
    }

    private bool RelaxColumns(int rows, int cols, int[][] rank)
    {
        var changed = false;

        for (var c = 0; c < cols; c++)
        {
            changed |= RelaxLine(rows, i => _matrix[i][c], i => new RankCell(rank[i], c));
        }

        return changed;
    }

    private static bool RelaxLine(int count, Func<int, int> valueAt, Func<int, RankCell> cellAt)
    {
        var changed = false;

        for (var i = 0; i < count; i++)
        {
            for (var j = i + 1; j < count; j++)
            {
                changed |= RelaxPair(valueAt(i), valueAt(j), cellAt(i), cellAt(j));
            }
        }

        return changed;
    }

    private static bool RelaxPair(int valueA, int valueB, RankCell cellA, RankCell cellB)
    {
        if (valueA < valueB && cellA.Row[cellA.Col] >= cellB.Row[cellB.Col])
        {
            cellB.Row[cellB.Col] = cellA.Row[cellA.Col] + 1;
            return true;
        }

        if (valueB < valueA && cellB.Row[cellB.Col] >= cellA.Row[cellA.Col])
        {
            cellA.Row[cellA.Col] = cellB.Row[cellB.Col] + 1;
            return true;
        }

        if (valueA == valueB && cellA.Row[cellA.Col] != cellB.Row[cellB.Col])
        {
            var merged = Math.Max(cellA.Row[cellA.Col], cellB.Row[cellB.Col]);
            cellA.Row[cellA.Col] = merged;
            cellB.Row[cellB.Col] = merged;
            return true;
        }

        return false;
    }

    [Benchmark]
    public int[][] DisjointSetRanking()
    {
        var rows = _matrix.Length;
        var cols = _matrix[0].Length;
        var cells = BuildCells(rows, cols);

        SortCellsByValue(cells);

        var result = CreateEmptyResult(rows, cols);
        var context = new RankingBatchContext(cells, rows, result, new int[rows], new int[cols]);

        ProcessAllBatches(context);

        return result;
    }

    private (int Value, int Row, int Col)[] BuildCells(int rows, int cols)
    {
        var cells = new (int Value, int Row, int Col)[rows * cols];

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                cells[(r * cols) + c] = (_matrix[r][c], r, c);
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

    private static void ProcessAllBatches(RankingBatchContext context)
    {
        var index = 0;

        while (index < context.Cells.Length)
        {
            index = ProcessNextBatch(context, index);
        }
    }

    private static int ProcessNextBatch(RankingBatchContext context, int index)
    {
        var end = index;

        while (end < context.Cells.Length && context.Cells[end].Value == context.Cells[index].Value)
        {
            end++;
        }

        AssignBatchRanks(context, index, end);

        return end;
    }

    private static void AssignBatchRanks(RankingBatchContext context, int start, int end)
    {
        var cells = context.Cells;
        var components = new DisjointSet(context.Rows + context.ColRank.Length);

        for (var i = start; i < end; i++)
        {
            components.Union(cells[i].Row, context.Rows + cells[i].Col);
        }

        var bestByRoot = new HashMap<int, int>();

        for (var i = start; i < end; i++)
        {
            UpdateBestForCell(context, components, bestByRoot, cells[i]);
        }

        for (var i = start; i < end; i++)
        {
            AssignCellRank(context, components, bestByRoot, cells[i]);
        }
    }

    private static void UpdateBestForCell(
        RankingBatchContext context, DisjointSet components, HashMap<int, int> bestByRoot, (int Value, int Row, int Col) cell)
    {
        var (_, row, col) = cell;
        var root = components.Find(row);
        var candidate = Math.Max(context.RowRank[row], context.ColRank[col]);

        if (!bestByRoot.TryGetValue(root, out var best) || candidate > best)
        {
            bestByRoot.Set(root, candidate);
        }
    }

    private static void AssignCellRank(
        RankingBatchContext context, DisjointSet components, HashMap<int, int> bestByRoot, (int Value, int Row, int Col) cell)
    {
        var (_, row, col) = cell;
        var root = components.Find(row);
        bestByRoot.TryGetValue(root, out var best);
        var rank = best + 1;

        context.Result[row][col] = rank;
        context.RowRank[row] = rank;
        context.ColRank[col] = rank;
    }

    private readonly record struct RankCell(int[] Row, int Col);

    private readonly record struct RankingBatchContext(
        (int Value, int Row, int Col)[] Cells, int Rows, int[][] Result, int[] RowRank, int[] ColRank);
}
