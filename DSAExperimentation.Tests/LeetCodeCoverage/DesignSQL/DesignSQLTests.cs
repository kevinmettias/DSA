using DSAExperimentation.LeetCode.DesignSQL;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignSQL;

// Harness only. Both strategies are DesignSQLSolution's - this file replays
// LeetCode's published call sequence against each ISqlStrategy implementation via a
// small operation script, so a failure still names the strategy that broke even
// though the "input" here is a constructor plus a sequence of mutating calls rather
// than a single argument tuple. SqlOp.Apply is pure dispatch (which method to call
// with which arguments) - no row-id or table-lookup logic of its own.
public sealed class DesignSQLTests
{
    public static TheoryData<string[], int[], SqlOp[], string?[]> Examples =>
        new()
        {
            // LeetCode's published example: insert two rows into "two", read a cell
            // of the first, delete the first, then read a cell of the second - the
            // deleted id is gone but the second row keeps the id it was given.
            {
                ["one", "two", "three"],
                [2, 3, 1],
                [
                    SqlOp.InsertRow("two", ["first", "second", "third"]),
                    SqlOp.SelectCell("two", 1, 3),
                    SqlOp.InsertRow("two", ["fourth", "fifth", "sixth"]),
                    SqlOp.DeleteRow("two", 1),
                    SqlOp.SelectCell("two", 2, 2),
                ],
                [null, "third", null, null, "fifth"]
            },

            // Ids are handed out in order and a deleted one is never reused: the
            // insert after the delete gets 3, not the freed 1.
            {
                ["t"],
                [1],
                [
                    SqlOp.InsertRow("t", ["a"]),
                    SqlOp.InsertRow("t", ["b"]),
                    SqlOp.DeleteRow("t", 1),
                    SqlOp.InsertRow("t", ["c"]),
                    SqlOp.SelectCell("t", 2, 1),
                    SqlOp.SelectCell("t", 3, 1),
                ],
                [null, null, null, null, "b", "c"]
            },

            // Separate tables keep separate id counters and separate rows, so both
            // hold a row 1 and neither sees the other's.
            {
                ["one", "two"],
                [1, 1],
                [
                    SqlOp.InsertRow("one", ["alpha"]),
                    SqlOp.InsertRow("two", ["beta"]),
                    SqlOp.SelectCell("one", 1, 1),
                    SqlOp.SelectCell("two", 1, 1),
                ],
                [null, null, "alpha", "beta"]
            },

            // Every column of one row, read first-to-last and last-to-first, so a
            // strategy that mixed up the 1-based column id cannot pass on symmetry.
            {
                ["grid"],
                [3],
                [
                    SqlOp.InsertRow("grid", ["x", "y", "z"]),
                    SqlOp.SelectCell("grid", 1, 1),
                    SqlOp.SelectCell("grid", 1, 2),
                    SqlOp.SelectCell("grid", 1, 3),
                    SqlOp.SelectCell("grid", 1, 3),
                    SqlOp.SelectCell("grid", 1, 1),
                ],
                [null, "x", "y", "z", "z", "x"]
            },

            // Deleting the last row still leaves the earlier ones addressable, and
            // the counter keeps climbing past every id ever issued.
            {
                ["log"],
                [1],
                [
                    SqlOp.InsertRow("log", ["one"]),
                    SqlOp.InsertRow("log", ["two"]),
                    SqlOp.InsertRow("log", ["three"]),
                    SqlOp.DeleteRow("log", 3),
                    SqlOp.DeleteRow("log", 2),
                    SqlOp.InsertRow("log", ["four"]),
                    SqlOp.SelectCell("log", 1, 1),
                    SqlOp.SelectCell("log", 4, 1),
                ],
                [null, null, null, null, null, null, "one", "four"]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SqlByListScan_LeetCodeExamples_ReadsTheCellOfTheLiveRow(
        string[] names, int[] columns, SqlOp[] operations, string?[] expected) =>
        RunScript(new DesignSQLSolution.SqlByListScan(names, columns), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void SqlByHashMapTables_LeetCodeExamples_ReadsTheCellOfTheLiveRow(
        string[] names, int[] columns, SqlOp[] operations, string?[] expected) =>
        RunScript(new DesignSQLSolution.SqlByHashMapTables(names, columns), operations, expected);

    private static void RunScript(DesignSQLSolution.ISqlStrategy strategy, SqlOp[] operations, string?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(strategy));
        }
    }
}
