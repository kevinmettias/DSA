namespace DSAExperimentation.LeetCode.Solutions.SubrectangleQueries;

// LeetCode's updateSubrectangle call, which lists the rectangle's corners as four arguments where
// ISubrectangleQueries takes them as one SubrectangleBounds (ARCHITECTURE 17.13).
internal static class SubrectangleQueriesCalls
{
    public static void UpdateSubrectangle(SubrectangleQueriesSolution.ISubrectangleQueries queries, int row1, int col1, int row2, int col2, int newValue) =>
        queries.UpdateSubrectangle(new SubrectangleQueriesSolution.SubrectangleBounds(row1, col1, row2, col2), newValue);
}
