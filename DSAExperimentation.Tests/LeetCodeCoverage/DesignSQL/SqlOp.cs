using DSAExperimentation.LeetCode.DesignSQL;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignSQL;

// One call in an SQL script: which method to invoke and with what arguments. Pure
// dispatch, built via the named factories below so a script (like Examples in
// DesignSQLTests) reads like the LeetCode call sequence it replays.
public readonly record struct SqlOp(SqlOp.SqlCall call, string name, string[] row, int rowId, int columnId)
{
    public static SqlOp InsertRow(string name, string[] row)
        => new(SqlCall.Insert, name, row, rowId: 0, columnId: 0);

    public static SqlOp DeleteRow(string name, int rowId)
        => new(SqlCall.Delete, name, [], rowId, columnId: 0);

    public static SqlOp SelectCell(string name, int rowId, int columnId)
        => new(SqlCall.Select, name, [], rowId, columnId);

    // null for the two void calls, the read cell for selectCell - so a script
    // runner can assert against one expected value per operation uniformly.
    // Internal, not public: ISqlStrategy is internal to DesignSQLSolution, and only
    // this same assembly's RunScript ever calls Apply.
    internal string? Apply(DesignSQLSolution.ISqlStrategy strategy)
    {
        switch (call)
        {
            case SqlCall.Insert:
                strategy.InsertRow(name, row);
                return null;
            case SqlCall.Delete:
                strategy.DeleteRow(name, rowId);
                return null;
            default:
                return strategy.SelectCell(name, rowId, columnId);
        }
    }

    public enum SqlCall
    {
        Insert,
        Delete,
        Select,
    }
}
