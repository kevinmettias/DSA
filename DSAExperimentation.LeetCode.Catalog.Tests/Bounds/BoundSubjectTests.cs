using DSAExperimentation.LeetCode.Catalog.Bounds;
using DSAExperimentation.LeetCode.Catalog.Questions;

namespace DSAExperimentation.LeetCode.Catalog.Tests.Bounds;

public sealed partial class BoundSubjectTests
{
    private static readonly IReadOnlyList<LeetCodeParameter> Parameters =
        [new LeetCodeParameter("grid", "integer[][]"), new LeetCodeParameter("s", "string"), new LeetCodeParameter("k", "integer")];

    private static readonly LeetCodeCase Case = new() { Inputs = ["[[1,2],[3,4,5]]", "\"abc\"", "7"] };

    public static TheoryData<string, double[]> Readable =>
        new()
        {
            { "k", [7] },
            { "grid.length", [2] },
            { "grid[i].length", [2, 3] },
            { "grid[i][j]", [1, 2, 3, 4, 5] },
            { "grid[i][2]", [5] },
            { "s.length", [3] },
        };

    public static TheoryData<string> Unreadable =>
        new()
        {
            "Node.val",
            "s[i]",
            "grid.length.length",
            "k.size",
        };

    [Theory]
    [MemberData(nameof(Readable))]
    public void ValuesOf_SubjectTheInputsCanAnswer_ReadsEveryValueItNames(string subject, double[] expected)
    {
        var values = BoundSubject.ValuesOf(subject, Case, Parameters);

        Assert.Equal(expected, values);
    }

    [Theory]
    [MemberData(nameof(Unreadable))]
    public void ValuesOf_SubjectTheInputsCannotAnswer_ReturnsNull(string subject)
    {
        var values = BoundSubject.ValuesOf(subject, Case, Parameters);

        Assert.Null(values);
    }
}
