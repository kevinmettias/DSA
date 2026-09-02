using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Delete Greatest Value in Each Row (LC 2500): the problem's own repeated
// simulation (find-and-remove the current max of each row, `Columns` times, via a
// plain O(Columns) scan per round - O(Rows*Columns^2) total) vs. this repo's
// MergeSort over ArrayIndexedSequence sorting each row once (O(Rows*Columns*log
// Columns)) followed by a single column-wise max pass. Both compute the same sum;
// RepeatedRowMaxScan is forced through its full quadratic-per-row cost since
// nothing short-circuits a full round of "find this row's current max."
[MemoryDiagnoser]
public class DeleteGreatestValueInEachRowBenchmarks
{
    private const int RandomSeed = 2500;
    private const int ValueBound = 100_000;

    [Params(50, 400)]
    public int Columns;

    private const int Rows = 20;

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _grid = Enumerable.Range(0, Rows)
            .Select(_ => Enumerable.Range(0, Columns).Select(_ => random.Next(ValueBound)).ToArray())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RepeatedRowMaxScan()
    {
        var rows = _grid.Select(row => row.ToArray()).ToArray();
        var removed = new bool[Rows, Columns];
        var sum = 0;

        for (var round = 0; round < Columns; round++)
        {
            var roundMax = 0;

            for (var row = 0; row < Rows; row++)
            {
                roundMax = Math.Max(roundMax, RemoveRowMax(rows[row], removed, row));
            }

            sum += roundMax;
        }

        return sum;
    }

    private static int RemoveRowMax(int[] row, bool[,] removed, int rowIndex)
    {
        var maxIndex = -1;

        for (var column = 0; column < row.Length; column++)
        {
            if (!removed[rowIndex, column] && (maxIndex < 0 || row[column] > row[maxIndex]))
            {
                maxIndex = column;
            }
        }

        removed[rowIndex, maxIndex] = true;
        return row[maxIndex];
    }

    [Benchmark]
    public int MergeSortColumnMax()
    {
        var rows = _grid.Select(row => row.ToArray()).ToArray();

        foreach (var row in rows)
        {
            MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(row));
        }

        var sum = 0;

        for (var column = 0; column < Columns; column++)
        {
            var columnMax = 0;

            foreach (var row in rows)
            {
                columnMax = Math.Max(columnMax, row[column]);
            }

            sum += columnMax;
        }

        return sum;
    }
}
