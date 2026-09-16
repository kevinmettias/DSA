using DSAExperimentation.LeetCode.ExpressionAddOperators;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ExpressionAddOperators;

// Harness only: both strategies live in ExpressionAddOperatorsSolution and are
// asserted against the same examples, including a leading-zero case and a
// no-solution case.
public sealed partial class ExpressionAddOperatorsTests
{
    public static TheoryData<string, int, string[]> Examples =>
        new()
        {
            { "123", 6, ["1+2+3", "1*2*3"] },
            { "232", 8, ["2*3+2", "2+3*2"] },
            { "105", 5, ["1*0+5", "10-5"] },
            { "00", 0, ["0*0", "0+0", "0-0"] },
            { "1", 5, [] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void AddOperatorsByBacktracking_LeetCodeExamples_ReturnsEveryExpressionEvaluatingToTarget(
        string num, int target, string[] expected)
    {
        var actual = ExpressionAddOperatorsSolution.AddOperatorsByBacktracking(num, target);
        AssertSameExpressions(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void AddOperatorsByDepthFirstSearchTraverse_LeetCodeExamples_ReturnsEveryExpressionEvaluatingToTarget(
        string num, int target, string[] expected)
    {
        var actual = ExpressionAddOperatorsSolution.AddOperatorsByDepthFirstSearchTraverse(num, target);
        AssertSameExpressions(expected, actual);
    }

    private static void AssertSameExpressions(string[] expected, List<string> actual)
    {
        var expectedSorted = expected.OrderBy(x => x, StringComparer.Ordinal);
        var actualSorted = actual.OrderBy(x => x, StringComparer.Ordinal);
        Assert.Equal(expectedSorted, actualSorted);
    }
}
