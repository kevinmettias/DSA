using DSAExperimentation.LeetCode.DesignSQL;

namespace DSAExperimentation.LeetCode.Tests.DesignSQL;

// One call in an SQL script: which method to invoke and with what arguments. Pure
// dispatch, built via the named factories below - LeetCode's own ins, rmv, sel and
// exp - so a script (like Examples in DesignSQLSolutionTests) reads like the LeetCode
// call sequence it replays.
public readonly record struct SqlOp(SqlOp.SqlCall call, string name, string[] row, int rowId, int columnId)
{
    public static SqlOp Ins(string name, string[] row)
        => new(SqlCall.Ins, name, row, rowId: 0, columnId: 0);

    public static SqlOp Rmv(string name, int rowId)
        => new(SqlCall.Rmv, name, [], rowId, columnId: 0);

    public static SqlOp Sel(string name, int rowId, int columnId)
        => new(SqlCall.Sel, name, [], rowId, columnId);

    public static SqlOp Exp(string name)
        => new(SqlCall.Exp, name, [], rowId: 0, columnId: 0);

    // What LeetCode's judge prints for the call: ins's bool, null for the void rmv,
    // sel's cell and exp's rows - each boxed as its own type, so a script runner can
    // assert against one expected value per operation uniformly. Each call goes to
    // the strategy method named for its operation (ins to TryInsert, and so on).
    // Internal, not public: ISqlStrategy is internal to DesignSQLSolution, and only
    // this same assembly's RunScript ever calls Apply.
    internal object? Apply(DesignSQLSolution.ISqlStrategy strategy)
    {
        switch (call)
        {
            case SqlCall.Ins:
                return strategy.TryInsert(name, row);
            case SqlCall.Rmv:
                strategy.Remove(name, rowId);
                return null;
            case SqlCall.Sel:
                return strategy.Select(name, rowId, columnId);
            default:
                return strategy.Export(name);
        }
    }

    public enum SqlCall
    {
        Ins,
        Rmv,
        Sel,
        Exp,
    }
}
