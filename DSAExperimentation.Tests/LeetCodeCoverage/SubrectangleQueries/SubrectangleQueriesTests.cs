using DSAExperimentation.LeetCode.SubrectangleQueries;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SubrectangleQueries;

// Harness only: both strategies are SubrectangleQueriesSolution's. LeetCode's own
// shape here is a stateful object across a sequence of calls, so Examples encodes a
// call script instead of a single argument tuple - the same shape
// DesignCircularQueueTests already uses for its own instance-API problem.
// SubrectangleQueryOp.Apply returns null for UpdateSubrectangle, which LeetCode's
// own judge output also reports as null, and the read value for GetValue.
public sealed partial class SubrectangleQueriesTests
{
    public static TheoryData<int[][], SubrectangleQueryOp[], int?[]> Examples =>
        new()
        {
            // LeetCode's published example 1.
            {
                [[1, 2, 1], [4, 3, 4], [3, 2, 1], [1, 1, 1]],
                [
                    SubrectangleQueryOp.GetValue(0, 2),
                    SubrectangleQueryOp.Update(new SubrectangleQueriesSolution.SubrectangleBounds(0, 0, 3, 2), 5),
                    SubrectangleQueryOp.GetValue(0, 2),
                    SubrectangleQueryOp.GetValue(3, 1),
                    SubrectangleQueryOp.Update(new SubrectangleQueriesSolution.SubrectangleBounds(3, 0, 3, 2), 10),
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
                    SubrectangleQueryOp.Update(new SubrectangleQueriesSolution.SubrectangleBounds(0, 0, 2, 2), 4),
                    SubrectangleQueryOp.GetValue(0, 0),
                    SubrectangleQueryOp.Update(new SubrectangleQueriesSolution.SubrectangleBounds(0, 0, 2, 2), 5),
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
                    SubrectangleQueryOp.Update(new SubrectangleQueriesSolution.SubrectangleBounds(0, 0, 2, 2), 100),
                    SubrectangleQueryOp.GetValue(2, 2),
                ],
                [1, null, 100]
            },

            // A single-cell update leaves every neighbouring cell alone.
            {
                [[1, 2, 3], [4, 5, 6], [7, 8, 9]],
                [
                    SubrectangleQueryOp.Update(new SubrectangleQueriesSolution.SubrectangleBounds(1, 1, 1, 1), 0),
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
                    SubrectangleQueryOp.Update(new SubrectangleQueriesSolution.SubrectangleBounds(0, 0, 0, 0), 3),
                    SubrectangleQueryOp.GetValue(0, 0),
                ],
                [7, null, 3]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SubrectangleQueriesByArrayBacked_LeetCodeExamples_MatchesExpectedSequence(
        int[][] rectangle, SubrectangleQueryOp[] operations, int?[] expected) =>
        Assert.Equal(expected, RunScript(new SubrectangleQueriesSolution.SubrectangleQueriesByArrayBacked(rectangle), operations));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SubrectangleQueriesByDynamicArrayBacked_LeetCodeExamples_MatchesExpectedSequence(
        int[][] rectangle, SubrectangleQueryOp[] operations, int?[] expected) =>
        Assert.Equal(expected, RunScript(new SubrectangleQueriesSolution.SubrectangleQueriesByDynamicArrayBacked(rectangle), operations));

    private static int?[] RunScript(
        SubrectangleQueriesSolution.ISubrectangleQueries queries, SubrectangleQueryOp[] operations) =>
        [.. operations.Select(operation => operation.Apply(queries))];

    // One call in a SubrectangleQueries script: which operation to invoke and with
    // what arguments. Pure dispatch, built via the named factories below so a script
    // (like Examples above) reads like the LeetCode call sequence it replays. The
    // overwrite takes the solution's own SubrectangleBounds rather than four adjacent
    // coordinates, so the rectangle it names is one value here as it is there.
    public readonly record struct SubrectangleQueryOp
    {
        private readonly SubrectangleOperation _operation;
        private readonly SubrectangleQueriesSolution.SubrectangleBounds _bounds;
        private readonly int _value;

        private SubrectangleQueryOp(
            SubrectangleOperation operation, SubrectangleQueriesSolution.SubrectangleBounds bounds, int value)
        {
            _operation = operation;
            _bounds = bounds;
            _value = value;
        }

        // Internal rather than public for the same reason Apply is: SubrectangleBounds is
        // internal, and a public method may not take a parameter type less accessible
        // than itself (CS0051). The script above is in this same assembly, so this is
        // the widest accessibility the factory can have.
        internal static SubrectangleQueryOp Update(
            SubrectangleQueriesSolution.SubrectangleBounds bounds, int newValue) =>
            new(SubrectangleOperation.UpdateSubrectangle, bounds, newValue);

        public static SubrectangleQueryOp GetValue(int row, int col) =>
            new(SubrectangleOperation.GetValue, new SubrectangleQueriesSolution.SubrectangleBounds(row, col, row, col), 0);

        // null for UpdateSubrectangle, which returns nothing; the cell's value for
        // GetValue - so one script runner can assert a single expected entry per
        // operation uniformly.
        internal int? Apply(SubrectangleQueriesSolution.ISubrectangleQueries queries)
        {
            if (_operation == SubrectangleOperation.GetValue)
            {
                return queries.GetValue(_bounds.Row1, _bounds.Col1);
            }

            queries.UpdateSubrectangle(_bounds, _value);
            return null;
        }
    }

    // Which of the two scripted operations an entry replays. A named type rather
    // than a bare `bool` on the constructor, so the entry's own discriminant says
    // what it is instead of a position the reader has to remember.
    private enum SubrectangleOperation
    {
        GetValue,
        UpdateSubrectangle,
    }
}
