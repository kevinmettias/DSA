using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.LeetCode.SubrectangleQueries;

// LeetCode 1476. Subrectangle Queries: a mutable matrix with two operations -
// overwrite every cell of an axis-aligned subrectangle, and read one cell back.
//
// This is a design problem - LeetCode's own shape is a stateful object with a
// constructor and two operations, not a single return value - so "every strategy
// for the problem" (ARCHITECTURE.md §17.3) takes the form of two classes
// implementing the shared ISubrectangleQueries surface below, the same shape
// DesignCircularQueueSolution uses for its own instance-API problem (LC 622).
//
// Both strategies run the identical brute-force nested overwrite that LeetCode's
// own constraints are sized for (<=100x100 grid, <=500 queries); there is no
// smarter algorithm to reach for here, only a backing-store choice - a plain
// jagged int[][] versus this repo's own DynamicArray<T>, nested once for the row
// list and once per row, the same "compose the array Representation primitive
// directly" move Stack<T> already makes (ARCHITECTURE.md §4.1).
internal static class SubrectangleQueriesSolution
{
    // The rectangle one UpdateSubrectangle call overwrites, inclusive at both
    // corners - LeetCode passes the four coordinates positionally, which is
    // exactly the transposable-argument hazard a named quadruple removes.
    internal readonly record struct SubrectangleBounds(int Row1, int Col1, int Row2, int Col2);

    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one call script against either strategy without
    // restating it.
    internal interface ISubrectangleQueries
    {
        void UpdateSubrectangle(SubrectangleBounds bounds, int newValue);

        int GetValue(int row, int col);
    }

    // The textbook baseline this composition has to justify itself against: a
    // plain BCL jagged int[][], deliberately written without this repo's
    // DynamicArray<T>.
    internal sealed class SubrectangleQueriesByArrayBacked : ISubrectangleQueries
    {
        private readonly int[][] _rectangle;

        // LeetCode's own constructor shape.
        public SubrectangleQueriesByArrayBacked(int[][] rectangle)
        {
            _rectangle = new int[rectangle.Length][];

            for (var r = 0; r < rectangle.Length; r++)
            {
                _rectangle[r] = (int[])rectangle[r].Clone();
            }
        }

        // A zero-filled grid of a given shape, so a harness measuring repeated
        // overwrites gets a fresh backing store per invocation instead of
        // carrying the previous invocation's mutations forward.
        public SubrectangleQueriesByArrayBacked(int rows, int columns)
        {
            _rectangle = new int[rows][];

            for (var r = 0; r < rows; r++)
            {
                _rectangle[r] = new int[columns];
            }
        }

        public void UpdateSubrectangle(SubrectangleBounds bounds, int newValue)
        {
            for (var r = bounds.Row1; r <= bounds.Row2; r++)
            {
                for (var c = bounds.Col1; c <= bounds.Col2; c++)
                {
                    _rectangle[r][c] = newValue;
                }
            }
        }

        public int GetValue(int row, int col) => _rectangle[row][col];
    }

    // The composed answer: this repo's own DynamicArray<T> as the backing store,
    // nested once for the row list and once per row.
    internal sealed class SubrectangleQueriesByDynamicArrayBacked : ISubrectangleQueries
    {
        private readonly DynamicArray<DynamicArray<int>> _rectangle = new();

        public SubrectangleQueriesByDynamicArrayBacked(int[][] rectangle)
        {
            foreach (var sourceRow in rectangle)
            {
                var row = new DynamicArray<int>();

                foreach (var value in sourceRow)
                {
                    row.Add(value);
                }

                _rectangle.Add(row);
            }
        }

        public SubrectangleQueriesByDynamicArrayBacked(int rows, int columns)
        {
            for (var r = 0; r < rows; r++)
            {
                var row = new DynamicArray<int>();

                for (var c = 0; c < columns; c++)
                {
                    row.Add(0);
                }

                _rectangle.Add(row);
            }
        }

        public void UpdateSubrectangle(SubrectangleBounds bounds, int newValue)
        {
            for (var r = bounds.Row1; r <= bounds.Row2; r++)
            {
                var row = _rectangle.Get(r);

                for (var c = bounds.Col1; c <= bounds.Col2; c++)
                {
                    row.Set(c, newValue);
                }
            }
        }

        public int GetValue(int row, int col) => _rectangle.Get(row).Get(col);
    }
}
