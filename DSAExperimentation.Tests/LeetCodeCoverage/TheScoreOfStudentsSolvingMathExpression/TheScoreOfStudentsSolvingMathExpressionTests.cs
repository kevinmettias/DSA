using DSAExperimentation.LeetCode.TheScoreOfStudentsSolvingMathExpression;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TheScoreOfStudentsSolvingMathExpression;

// Harness only: both strategies live in TheScoreOfStudentsSolvingMathExpressionSolution
// and are asserted against the same examples, so the un-memoized interval search - which
// used to exist only as this problem's benchmark baseline, measured but never asserted -
// is held to the same answers as the memoized one.
//
// The three published LeetCode examples are joined by the "2+3*4" cases the pre-migration
// test carried. That expression has exactly two full parenthesizations: (2+3)*4 = 20 and
// 2+(3*4) = 14. Standard precedence gives 14, so 14 scores 5, the other achievable value
// 20 scores 2, and 99 - achievable under no parenthesization at all - scores 0.
public sealed class TheScoreOfStudentsSolvingMathExpressionTests
{
    public static TheoryData<string, int[], int> Examples =>
        new()
        {
            { "7+3*1*2", [20, 13, 42], 7 },
            { "3+5*2", [13, 0, 10, 13, 13, 16, 16], 19 },
            { "6+0*1", [12, 9, 6, 4, 8, 6], 10 },
            { "2+3*4", [14], 5 },
            { "2+3*4", [20], 2 },
            { "2+3*4", [99], 0 },
            { "2+3*4", [14, 20, 99], 7 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ScoreOfStudentsByUnmemoizedRecursion_LeetCodeExamples_ReturnsTotalScore(
        string expression, int[] answers, int expected) =>
        Assert.Equal(
            expected,
            TheScoreOfStudentsSolvingMathExpressionSolution.ScoreOfStudentsByUnmemoizedRecursion(expression, answers));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ScoreOfStudentsByMemoizedIntervals_LeetCodeExamples_ReturnsTotalScore(
        string expression, int[] answers, int expected) =>
        Assert.Equal(
            expected,
            TheScoreOfStudentsSolvingMathExpressionSolution.ScoreOfStudentsByMemoizedIntervals(expression, answers));
}
