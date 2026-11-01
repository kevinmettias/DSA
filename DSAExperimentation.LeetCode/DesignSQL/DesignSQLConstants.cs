namespace DSAExperimentation.LeetCode.DesignSQL;

// The values LC 2408 fixes, declared once so DesignSQLSolution and its benchmark read the same copy
// instead of each restating it.
internal static class DesignSQLConstants
{
    // The id of a table's first row; each later one is one greater than the last
    // inserted, removed or not. Column ids count from 1 the same way.
    internal const int FirstId = 1;
}
