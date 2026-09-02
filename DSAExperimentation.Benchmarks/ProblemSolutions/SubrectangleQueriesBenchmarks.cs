using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Subrectangle Queries (LC 1476): a plain jagged int[][] backing store vs. this
// repo's own DynamicArray<T>, nested once for the row list and once per row - the
// same "compose the array Representation primitive directly" move Stack<T> and
// DesignCircularQueueBenchmarks's DequeBacked already make. Both variants run the
// identical brute-force nested-loop overwrite LeetCode's own constraints are sized
// for (<=100x100 grid, <=500 queries); there is no faster algorithm to compare here,
// only the backing-store choice.
[MemoryDiagnoser]
public class SubrectangleQueriesBenchmarks
{
    private const int QueryCount = 200;

    [Params(20, 100)]
    public int Size;

    private (int Row1, int Col1, int Row2, int Col2, int Value)[] _updates = null!;

    private readonly record struct Rectangle(int Row1, int Col1, int Row2, int Col2);

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _updates = new (int, int, int, int, int)[QueryCount];

        for (var i = 0; i < QueryCount; i++)
        {
            var row1 = random.Next(0, Size);
            var col1 = random.Next(0, Size);
            var row2 = random.Next(row1, Size);
            var col2 = random.Next(col1, Size);
            _updates[i] = (row1, col1, row2, col2, i);
        }
    }

    [Benchmark(Baseline = true)]
    public int ArrayBacked()
    {
        var queries = new ArraySubrectangleQueries(Size);

        foreach (var (row1, col1, row2, col2, value) in _updates)
        {
            queries.UpdateSubrectangle(new Rectangle(row1, col1, row2, col2), value);
        }

        return queries.GetValue(Size - 1, Size - 1);
    }

    [Benchmark]
    public int DynamicArrayBacked()
    {
        var queries = new DynamicArraySubrectangleQueries(Size);

        foreach (var (row1, col1, row2, col2, value) in _updates)
        {
            queries.UpdateSubrectangle(new Rectangle(row1, col1, row2, col2), value);
        }

        return queries.GetValue(Size - 1, Size - 1);
    }

    private sealed class ArraySubrectangleQueries
    {
        private readonly int[][] _rectangle;

        public ArraySubrectangleQueries(int size)
            => _rectangle = Enumerable.Range(0, size).Select(_ => new int[size]).ToArray();

        public void UpdateSubrectangle(Rectangle rect, int newValue)
        {
            for (var r = rect.Row1; r <= rect.Row2; r++)
            {
                for (var c = rect.Col1; c <= rect.Col2; c++)
                {
                    _rectangle[r][c] = newValue;
                }
            }
        }

        public int GetValue(int row, int col) => _rectangle[row][col];
    }

    private sealed class DynamicArraySubrectangleQueries
    {
        private readonly DynamicArray<DynamicArray<int>> _rectangle = new();

        public DynamicArraySubrectangleQueries(int size)
        {
            for (var r = 0; r < size; r++)
            {
                var row = new DynamicArray<int>();

                for (var c = 0; c < size; c++)
                {
                    row.Add(0);
                }

                _rectangle.Add(row);
            }
        }

        public void UpdateSubrectangle(Rectangle rect, int newValue)
        {
            for (var r = rect.Row1; r <= rect.Row2; r++)
            {
                var row = _rectangle.Get(r);

                for (var c = rect.Col1; c <= rect.Col2; c++)
                {
                    row.Set(c, newValue);
                }
            }
        }

        public int GetValue(int row, int col) => _rectangle.Get(row).Get(col);
    }
}
