namespace DSAExperimentation.LeetCode.Solutions.DesignSQL;

// LeetCode's four abbreviated calls on the database, which ISqlStrategy answers under whole names (ARCHITECTURE
// 17.13): ins is TryInsert, rmv is Remove, sel is Select and exp is Export.
internal static class DesignSQLCalls
{
    public static bool Ins(DesignSQLSolution.ISqlStrategy database, string name, string[] row) => database.TryInsert(name, row);

    public static void Rmv(DesignSQLSolution.ISqlStrategy database, string name, int rowId) => database.Remove(name, rowId);

    public static string Sel(DesignSQLSolution.ISqlStrategy database, string name, int rowId, int columnId) =>
        database.Select(name, rowId, columnId);

    public static string[] Exp(DesignSQLSolution.ISqlStrategy database, string name) => database.Export(name);
}
