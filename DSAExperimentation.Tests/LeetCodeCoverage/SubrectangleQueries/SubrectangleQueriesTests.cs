using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SubrectangleQueries;

// LeetCode 1476. Subrectangle Queries: a row-major matrix backed by this repo's own
// DynamicArray<T> - once for the row list, once per row - composed the same way
// Stack<T> composes DynamicArray<T> for its own backing store (ARCHITECTURE.md
// §4.1). UpdateSubrectangle is the brute-force nested overwrite LeetCode's own tiny
// constraints (<=100x100 grid, <=500 queries) are sized for; there is no smarter
// algorithm to reach for here, only a backing-store choice.
public sealed partial class SubrectangleQueriesTests
{
    [Fact]
    public void UpdateAndGetValue_LeetCodeExample_AppliesUpdatesToTheRightCells()
    {
        int[][] rectangle = [[1, 2, 1], [4, 3, 4], [3, 2, 1], [1, 1, 1]];
        var queries = new SubrectangleQueries(rectangle);

        var beforeAnyUpdate = queries.GetValue(0, 2);
        Assert.Equal(1, beforeAnyUpdate);

        queries.UpdateSubrectangle(new SubrectangleBounds(0, 0, 3, 2), 5);
        var row0Col2AfterFirstUpdate = queries.GetValue(0, 2);
        Assert.Equal(5, row0Col2AfterFirstUpdate);
        var row3Col1AfterFirstUpdate = queries.GetValue(3, 1);
        Assert.Equal(5, row3Col1AfterFirstUpdate);

        queries.UpdateSubrectangle(new SubrectangleBounds(3, 0, 3, 2), 10);
        var row3Col1AfterSecondUpdate = queries.GetValue(3, 1);
        Assert.Equal(10, row3Col1AfterSecondUpdate);
        var row0Col2AfterSecondUpdate = queries.GetValue(0, 2);
        Assert.Equal(5, row0Col2AfterSecondUpdate);
    }

    [Fact]
    public void UpdateSubrectangle_EntireGrid_OverwritesEveryCell()
    {
        int[][] rectangle = [[1, 1, 1], [2, 2, 2], [3, 3, 3]];
        var queries = new SubrectangleQueries(rectangle);

        var beforeUpdate = queries.GetValue(0, 0);
        Assert.Equal(1, beforeUpdate);

        queries.UpdateSubrectangle(new SubrectangleBounds(0, 0, 2, 2), 100);

        var afterFullGridUpdate = queries.GetValue(2, 2);
        Assert.Equal(100, afterFullGridUpdate);
    }

    private readonly record struct SubrectangleBounds(int Row1, int Col1, int Row2, int Col2);

    private sealed class SubrectangleQueries
    {
        private readonly DynamicArray<DynamicArray<int>> _rectangle = new();

        public SubrectangleQueries(int[][] rectangle)
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
