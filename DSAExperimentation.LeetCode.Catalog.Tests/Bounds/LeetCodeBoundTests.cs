using DSAExperimentation.LeetCode.Catalog.Bounds;
using DSAExperimentation.LeetCode.Catalog.Questions;

namespace DSAExperimentation.LeetCode.Catalog.Tests.Bounds;

public sealed partial class LeetCodeBoundTests
{
    private static readonly IReadOnlyList<LeetCodeParameter> TwoSumParameters =
        [new LeetCodeParameter("nums", "integer[]"), new LeetCodeParameter("target", "integer")];

    private static readonly IReadOnlyList<LeetCodeParameter> GridParameters =
        [new LeetCodeParameter("grid", "integer[][]"), new LeetCodeParameter("words", "string[]")];

    private static readonly IReadOnlyList<LeetCodeParameter> TwoScalarParameters =
        [new LeetCodeParameter("m", "integer"), new LeetCodeParameter("n", "integer")];

    public static TheoryData<LeetCodeBound, string[], LeetCodeBoundCheck> TwoSumVerdicts =>
        new()
        {
            { new(2, true, ["nums.length"], 10_000, true), ["[3,3]", "6"], LeetCodeBoundCheck.Satisfied },
            { new(2, true, ["nums.length"], 10_000, true), ["[3]", "6"], LeetCodeBoundCheck.Violated },
            { new(-10, true, ["nums[i]"], 10, true), ["[3,-11]", "6"], LeetCodeBoundCheck.Violated },
            { new(-10, true, ["target"], 10, true), ["[3,3]", "6"], LeetCodeBoundCheck.Satisfied },
            { new(-10, true, ["Node.val"], 10, true), ["[3,3]", "6"], LeetCodeBoundCheck.NotJudged },
            { new(1, true, ["nums.size"], 10, true), ["[3,3]", "6"], LeetCodeBoundCheck.NotJudged },
        };

    public static TheoryData<LeetCodeBound, string[], LeetCodeBoundCheck> GridVerdicts =>
        new()
        {
            { new(0, true, ["grid[i][j]"], 1, true), ["[[0,1],[1,0]]", "[]"], LeetCodeBoundCheck.Satisfied },
            { new(0, true, ["grid[i][j]"], 1, true), ["[[0,1],[2,0]]", "[]"], LeetCodeBoundCheck.Violated },
            { new(1, true, ["grid[i][0]"], 1, true), ["[[1,0],[1,9]]", "[]"], LeetCodeBoundCheck.Satisfied },
            { new(1, true, ["words[i].length"], 3, true), ["[]", "[\"ab\",\"abc\"]"], LeetCodeBoundCheck.Satisfied },
            { new(1, true, ["words[i].length"], 2, true), ["[]", "[\"ab\",\"abc\"]"], LeetCodeBoundCheck.Violated },
            { new(1, true, ["grid.length"], 2, true), ["[[0],[0],[0]]", "[]"], LeetCodeBoundCheck.Violated },
        };

    public static TheoryData<LeetCodeBound, double[], bool[]> RangeMemberships =>
        new()
        {
            { new(1, true, ["n"], 3, true), [0, 1, 2, 3, 4], [false, true, true, true, false] },
            { new(1, false, ["n"], 3, false), [1, 2, 3], [false, true, false] },
        };

    [Theory]
    [MemberData(nameof(RangeMemberships))]
    public void IsInRange_InclusiveAndExclusiveLimits_AdmitsTheEndsOnlyWhenInclusive(
        LeetCodeBound bound, double[] values, bool[] expected)
    {
        var memberships = values.Select(bound.IsInRange);

        Assert.Equal(expected, memberships);
    }

    [Theory]
    [MemberData(nameof(TwoSumVerdicts))]
    public void Check_ScalarAndArraySubjects_JudgesTheNamedInput(LeetCodeBound bound, string[] inputs, LeetCodeBoundCheck expected)
    {
        var verdict = bound.Check(new LeetCodeCase { Inputs = inputs }, TwoSumParameters);

        Assert.Equal(expected, verdict);
    }

    [Theory]
    [MemberData(nameof(GridVerdicts))]
    public void Check_NestedSubjects_JudgesEveryValueTheAccessorsReach(LeetCodeBound bound, string[] inputs, LeetCodeBoundCheck expected)
    {
        var verdict = bound.Check(new LeetCodeCase { Inputs = inputs }, GridParameters);

        Assert.Equal(expected, verdict);
    }

    [Fact]
    public void Check_TwoSubjectsOneOutOfRange_ReportsViolated()
    {
        var bound = new LeetCodeBound(1, true, ["m", "n"], 5, true);

        var verdict = bound.Check(new LeetCodeCase { Inputs = ["2", "9"] }, TwoScalarParameters);

        Assert.Equal(LeetCodeBoundCheck.Violated, verdict);
    }
}
