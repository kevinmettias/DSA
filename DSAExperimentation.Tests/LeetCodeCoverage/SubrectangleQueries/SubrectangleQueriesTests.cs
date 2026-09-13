using static DSAExperimentation.LeetCode.SubrectangleQueries.SubrectangleQueriesSolution;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SubrectangleQueries;

// Harness only: both strategies are SubrectangleQueriesSolution's. LeetCode's own
// shape here is a stateful object across a sequence of calls, so Examples encodes a
// call script instead of a single argument tuple - the same shape
// DesignCircularQueueTests already uses for its own instance-API problem.
// SubrectangleQueryOp.Apply returns null for UpdateSubrectangle, which LeetCode's
// own judge output also reports as null, and the read value for GetValue.
public sealed class SubrectangleQueriesTests
{
    public static TheoryData<int[][], SubrectangleQueryOp[], int?[]> Examples =>
        new()
        {
            // LeetCode's published example 1.
            {
                [[1, 2, 1], [4, 3, 4], [3, 2, 1], [1, 1, 1]],
                [
                    SubrectangleQueryOp.GetValue(0, 2),
                    SubrectangleQueryOp.Update(0, 0, 3, 2, 5),
                    SubrectangleQueryOp.GetValue(0, 2),
                    SubrectangleQueryOp.GetValue(3, 1),
                    SubrectangleQueryOp.Update(3, 0, 3, 2, 10),
                    SubrectangleQueryOp.GetValue(3, 1),
                    SubrectangleQueryOp.GetValue(0, 2),
                ],
                [1, null, 5, 5, null, 10, 5]
            },

            // LeetCode's published example 2: two overlapping full-grid
            // overwrites, so the second has to win at every cell.
            {
                [[1, 1, 1], [2, 2, 2], [3, 3, 3]],
                [
                    SubrectangleQueryOp.Update(0, 0, 2, 2, 4),
                    SubrectangleQueryOp.GetValue(0, 0),
                    SubrectangleQueryOp.Update(0, 0, 2, 2, 5),
                    SubrectangleQueryOp.GetValue(0, 0),
                    SubrectangleQueryOp.GetValue(2, 2),
                ],
                [null, 4, null, 5, 5]
            },

            // Whole-grid overwrite read at the far corner.
            {
                [[1, 1, 1], [2, 2, 2], [3, 3, 3]],
                [
                    SubrectangleQueryOp.GetValue(0, 0),
                    SubrectangleQueryOp.Update(0, 0, 2, 2, 100),
                    SubrectangleQueryOp.GetValue(2, 2),
                ],
                [1, null, 100]
            },

            // A single-cell update leaves every neighbouring cell alone.
            {
                [[1, 2, 3], [4, 5, 6], [7, 8, 9]],
                [
                    SubrectangleQueryOp.Update(1, 1, 1, 1, 0),
                    SubrectangleQueryOp.GetValue(1, 1),
                    SubrectangleQueryOp.GetValue(0, 1),
                    SubrectangleQueryOp.GetValue(1, 0),
                    SubrectangleQueryOp.GetValue(1, 2),
                    SubrectangleQueryOp.GetValue(2, 1),
                ],
                [null, 0, 2, 4, 6, 8]
            },

            // A 1x1 grid, the smallest input LeetCode's constraints allow.
            {
                [[7]],
                [
                    SubrectangleQueryOp.GetValue(0, 0),
                    SubrectangleQueryOp.Update(0, 0, 0, 0, 3),
                    SubrectangleQueryOp.GetValue(0, 0),
                ],
                [7, null, 3]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SubrectangleQueriesByArrayBacked_LeetCodeExamples_MatchesExpectedSequence(
        int[][] rectangle, SubrectangleQueryOp[] operations, int?[] expected) =>
        RunScript(new SubrectangleQueriesByArrayBacked(rectangle), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void SubrectangleQueriesByDynamicArrayBacked_LeetCodeExamples_MatchesExpectedSequence(
        int[][] rectangle, SubrectangleQueryOp[] operations, int?[] expected) =>
        RunScript(new SubrectangleQueriesByDynamicArrayBacked(rectangle), operations, expected);

    private static void RunScript(ISubrectangleQueries queries, SubrectangleQueryOp[] operations, int?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(queries));
        }
    }
}

// One call in a SubrectangleQueries script: which operation to invoke and with what
// arguments. Pure dispatch, built via the named factories below so a script (like
// Examples above) reads like the LeetCode call sequence it replays.
public readonly record struct SubrectangleQueryOp
{
    private readonly bool _isUpdate;
    private readonly SubrectangleBounds _bounds;
    private readonly int _value;

    private SubrectangleQueryOp(bool isUpdate, SubrectangleBounds bounds, int value)
    {
        _isUpdate = isUpdate;
        _bounds = bounds;
        _value = value;
    }

    public static SubrectangleQueryOp Update(int row1, int col1, int row2, int col2, int newValue) =>
        new(isUpdate: true, new SubrectangleBounds(row1, col1, row2, col2), newValue);

    public static SubrectangleQueryOp GetValue(int row, int col) =>
        new(isUpdate: false, new SubrectangleBounds(row, col, row, col), 0);

    // null for UpdateSubrectangle, which returns nothing; the cell's value for
    // GetValue - so one script runner can assert a single expected entry per
    // operation uniformly.
    internal int? Apply(ISubrectangleQueries queries)
    {
        if (!_isUpdate)
        {
            return queries.GetValue(_bounds.Row1, _bounds.Col1);
        }

        queries.UpdateSubrectangle(_bounds, _value);
        return null;
    }
}
