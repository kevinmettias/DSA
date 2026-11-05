using DSAExperimentation.LeetCode.Solutions.SubrectangleQueries;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SubrectangleQueries;

public sealed partial class SubrectangleQueriesCallsTests
{
    [Fact]
    public void UpdateSubrectangle_TopRow_SetsItsCellsAndNoOthers()
    {
        var queries = SubrectangleQueriesSolution.CreateByArrayBacked([[1, 2], [3, 4]]);

        SubrectangleQueriesCalls.UpdateSubrectangle(queries, 0, 0, 0, 1, 9);

        Assert.Equal((9, 9, 3), (queries.GetValue(0, 0), queries.GetValue(0, 1), queries.GetValue(1, 0)));
    }
}
