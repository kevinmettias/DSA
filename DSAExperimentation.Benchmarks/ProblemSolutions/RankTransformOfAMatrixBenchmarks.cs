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
    [Params(8, 20)]
    public int Size;

    private int[][] _matrix = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1632);
        _matrix = new int[Size][];

        for (var r = 0; r < Size; r++)
        {
            _matrix[r] = new int[Size];

            for (var c = 0; c < Size; c++)
            {
                _matrix[r][c] = random.Next(-100_000, 100_000);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int[][] IterativeRelaxation()
    {
        var rows = _matrix.Length;
        var cols = _matrix[0].Length;
        var rank = new int[rows][];

        for (var r = 0; r < rows; r++)
        {
            rank[r] = new int[cols];

            for (var c = 0; c < cols; c++)
            {
                rank[r][c] = 1;
            }
        }

        var changed = true;

        while (changed)
        {
            changed = false;
            changed |= RelaxRows(rows, cols, rank);
            changed |= RelaxColumns(rows, cols, rank);
        }

        return rank;
    }

    private bool RelaxRows(int rows, int cols, int[][] rank)
    {
        var changed = false;

        for (var r = 0; r < rows; r++)
        {
            for (var i = 0; i < cols; i++)
            {
                for (var j = i + 1; j < cols; j++)
                {
                    changed |= RelaxPair(_matrix[r][i], _matrix[r][j], rank[r], i, rank[r], j);
                }
            }
        }

        return changed;
    }

    private bool RelaxColumns(int rows, int cols, int[][] rank)
    {
        var changed = false;

        for (var c = 0; c < cols; c++)
        {
            for (var i = 0; i < rows; i++)
            {
                for (var j = i + 1; j < rows; j++)
                {
                    changed |= RelaxPair(_matrix[i][c], _matrix[j][c], rank[i], c, rank[j], c);
                }
            }
        }

        return changed;
    }

    private static bool RelaxPair(int valueA, int valueB, int[] rankRowA, int colA, int[] rankRowB, int colB)
    {
        if (valueA < valueB && rankRowA[colA] >= rankRowB[colB])
        {
            rankRowB[colB] = rankRowA[colA] + 1;
            return true;
        }

        if (valueB < valueA && rankRowB[colB] >= rankRowA[colA])
        {
            rankRowA[colA] = rankRowB[colB] + 1;
            return true;
        }

        if (valueA == valueB && rankRowA[colA] != rankRowB[colB])
        {
            var merged = Math.Max(rankRowA[colA], rankRowB[colB]);
            rankRowA[colA] = merged;
            rankRowB[colB] = merged;
            return true;
        }

        return false;
    }

    [Benchmark]
    public int[][] DisjointSetRanking()
    {
        var rows = _matrix.Length;
        var cols = _matrix[0].Length;
        var cells = new (int Value, int Row, int Col)[rows * cols];

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                cells[(r * cols) + c] = (_matrix[r][c], r, c);
            }
        }

        var byValue = Comparer<(int Value, int Row, int Col)>.Create((a, b) => a.Value.CompareTo(b.Value));
        MergeSort.Sort<(int Value, int Row, int Col), ArrayIndexedSequence<(int Value, int Row, int Col)>>(
            new ArrayIndexedSequence<(int Value, int Row, int Col)>(cells), byValue);

        var rowRank = new int[rows];
        var colRank = new int[cols];
        var result = new int[rows][];

        for (var r = 0; r < rows; r++)
        {
            result[r] = new int[cols];
        }

        var index = 0;

        while (index < cells.Length)
        {
            var end = index;

            while (end < cells.Length && cells[end].Value == cells[index].Value)
            {
                end++;
            }

            AssignBatchRanks(cells, index, end, rows, result, rowRank, colRank);
            index = end;
        }

        return result;
    }

    private static void AssignBatchRanks(
        (int Value, int Row, int Col)[] cells, int start, int end, int rows,
        int[][] result, int[] rowRank, int[] colRank)
    {
        var components = new DisjointSet(rows + colRank.Length);

        for (var i = start; i < end; i++)
        {
            components.Union(cells[i].Row, rows + cells[i].Col);
        }

        var bestByRoot = new HashMap<int, int>();

        for (var i = start; i < end; i++)
        {
            var (_, row, col) = cells[i];
            var root = components.Find(row);
            var candidate = Math.Max(rowRank[row], colRank[col]);

            if (!bestByRoot.TryGetValue(root, out var best) || candidate > best)
            {
                bestByRoot.Set(root, candidate);
            }
        }

        for (var i = start; i < end; i++)
        {
            var (_, row, col) = cells[i];
            var root = components.Find(row);
            bestByRoot.TryGetValue(root, out var best);
            var rank = best + 1;

            result[row][col] = rank;
            rowRank[row] = rank;
            colRank[col] = rank;
        }
    }
}
