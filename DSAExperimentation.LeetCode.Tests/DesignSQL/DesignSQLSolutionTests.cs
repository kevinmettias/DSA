using DSAExperimentation.LeetCode.DesignSQL;

namespace DSAExperimentation.LeetCode.Tests.DesignSQL;

// Harness only. Both strategies are DesignSQLSolution's - this file replays call
// scripts against each ISqlStrategy implementation via SqlOp, so a failure still
// names the strategy that broke even though the "input" here is a constructor plus a
// sequence of mutating calls rather than a single argument tuple. SqlOp.Apply is pure
// dispatch (which method to call with which arguments) - no row-id or table-lookup
// logic of its own. ins answers a bool, rmv nothing, sel a cell and exp an array of
// rows, so the expected sequence is object?[] and reads exactly like the published
// output, less the constructor's leading null.
public sealed partial class DesignSQLSolutionTests
{
    // LeetCode's two published examples, exactly as published.
    public static TheoryData<string[], int[], SqlOp[], object?[]> Examples =>
        new()
        {
            {
                ["one", "two", "three"],
                [2, 3, 1],
                [
                    SqlOp.Ins("two", ["first", "second", "third"]),
                    SqlOp.Sel("two", 1, 3),
                    SqlOp.Ins("two", ["fourth", "fifth", "sixth"]),
                    SqlOp.Exp("two"),
                    SqlOp.Rmv("two", 1),
                    SqlOp.Sel("two", 2, 2),
                    SqlOp.Exp("two"),
                ],
                [
                    true,
                    "third",
                    true,
                    new[] { "1,first,second,third", "2,fourth,fifth,sixth" },
                    null,
                    "fifth",
                    new[] { "2,fourth,fifth,sixth" },
                ]
            },
            {
                ["one", "two", "three"],
                [2, 3, 1],
                [
                    SqlOp.Ins("two", ["first", "second", "third"]),
                    SqlOp.Sel("two", 1, 3),
                    SqlOp.Rmv("two", 1),
                    SqlOp.Sel("two", 1, 2),
                    SqlOp.Ins("two", ["fourth", "fifth"]),
                    SqlOp.Ins("two", ["fourth", "fifth", "sixth"]),
                ],
                [true, "third", null, "<null>", false, true]
            },
        };

    // Scripts beyond the published ones, each expected value derived by hand from the
    // statement in the comment above it.
    public static TheoryData<string[], int[], SqlOp[], object?[]> HandDerivedScripts =>
        new()
        {
            // Example 2 made observable: the statement's "returns false without any
            // insertion" means the two-cell row on a three-column table uses up no id,
            // so the next row that fits gets id 2 - one greater than the last row
            // inserted, which was removed - and id 3 was never issued.
            {
                ["one", "two", "three"],
                [2, 3, 1],
                [
                    SqlOp.Ins("two", ["first", "second", "third"]),
                    SqlOp.Rmv("two", 1),
                    SqlOp.Ins("two", ["fourth", "fifth"]),
                    SqlOp.Ins("two", ["fourth", "fifth", "sixth"]),
                    SqlOp.Exp("two"),
                    SqlOp.Sel("two", 2, 3),
                    SqlOp.Sel("two", 3, 1),
                ],
                [true, null, false, true, new[] { "2,fourth,fifth,sixth" }, "sixth", "<null>"]
            },

            // A name that is not a table: ins refuses, rmv does nothing, sel answers
            // "<null>" and exp an empty array. The real table "one" is untouched, so its
            // first row is id 1; once that row is removed, "one" exports an empty
            // array too, though it is still a table.
            {
                ["one"],
                [2],
                [
                    SqlOp.Ins("zero", ["a", "b"]),
                    SqlOp.Rmv("zero", 1),
                    SqlOp.Sel("zero", 1, 1),
                    SqlOp.Exp("zero"),
                    SqlOp.Ins("one", ["a", "b"]),
                    SqlOp.Exp("one"),
                    SqlOp.Rmv("one", 1),
                    SqlOp.Exp("one"),
                ],
                [false, null, "<null>", Array.Empty<string>(), true, new[] { "1,a,b" }, null, Array.Empty<string>()]
            },

            // Invalid cells of a live row: column ids run 1 through 3 here and row ids
            // start at 1, so column 0, column 4 and row 0 are all "<null>", while
            // columns 1 and 3 read the first and last cell. A four-cell row is as
            // wrong a width as a two-cell one, so it is refused and not exported.
            {
                ["grid"],
                [3],
                [
                    SqlOp.Ins("grid", ["x", "y", "z"]),
                    SqlOp.Sel("grid", 1, 0),
                    SqlOp.Sel("grid", 1, 4),
                    SqlOp.Sel("grid", 0, 1),
                    SqlOp.Sel("grid", 1, 1),
                    SqlOp.Sel("grid", 1, 3),
                    SqlOp.Ins("grid", ["a", "b", "c", "d"]),
                    SqlOp.Exp("grid"),
                ],
                [true, "<null>", "<null>", "<null>", "x", "z", false, new[] { "1,x,y,z" }]
            },

            // Removing a row frees nothing: removing id 2 twice, or id 7 that was never
            // issued, changes nothing, and the next "log" row gets id 4, not the freed 2.
            // exp lists the live rows in id order, 1, 3, 4. "tag" keeps its own counter,
            // so its first row is id 1 although "log" has issued four.
            {
                ["log", "tag"],
                [1, 2],
                [
                    SqlOp.Ins("log", ["one"]),
                    SqlOp.Ins("log", ["two"]),
                    SqlOp.Ins("log", ["three"]),
                    SqlOp.Rmv("log", 2),
                    SqlOp.Rmv("log", 2),
                    SqlOp.Rmv("log", 7),
                    SqlOp.Ins("log", ["four"]),
                    SqlOp.Ins("tag", ["a", "b"]),
                    SqlOp.Exp("log"),
                    SqlOp.Exp("tag"),
                    SqlOp.Sel("log", 2, 1),
                    SqlOp.Sel("log", 4, 1),
                ],
                [
                    true,
                    true,
                    true,
                    null,
                    null,
                    null,
                    true,
                    true,
                    new[] { "1,one", "3,three", "4,four" },
                    new[] { "1,a,b" },
                    "<null>",
                    "four",
                ]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SqlByListScan_LeetCodeExamples_RepliesAsPublished(
        string[] names, int[] columns, SqlOp[] operations, object?[] expected) =>
        Assert.Equal(expected, RunScript(new DesignSQLSolution.SqlByListScan(names, columns), operations));

    [Theory]
    [MemberData(nameof(HandDerivedScripts))]
    public void SqlByListScan_HandDerivedScripts_RepliesAsDerived(
        string[] names, int[] columns, SqlOp[] operations, object?[] expected) =>
        Assert.Equal(expected, RunScript(new DesignSQLSolution.SqlByListScan(names, columns), operations));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SqlByHashMapTables_LeetCodeExamples_RepliesAsPublished(
        string[] names, int[] columns, SqlOp[] operations, object?[] expected) =>
        Assert.Equal(expected, RunScript(new DesignSQLSolution.SqlByHashMapTables(names, columns), operations));

    [Theory]
    [MemberData(nameof(HandDerivedScripts))]
    public void SqlByHashMapTables_HandDerivedScripts_RepliesAsDerived(
        string[] names, int[] columns, SqlOp[] operations, object?[] expected) =>
        Assert.Equal(expected, RunScript(new DesignSQLSolution.SqlByHashMapTables(names, columns), operations));

    private static object?[] RunScript(DesignSQLSolution.ISqlStrategy strategy, SqlOp[] operations) =>
        [.. operations.Select(operation => operation.Apply(strategy))];
}
