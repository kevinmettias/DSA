using DSAExperimentation.LeetCode.Solutions.TheScoreOfStudentsSolvingMathExpression;

namespace DSAExperimentation.LeetCode.Solutions.Tests.TheScoreOfStudentsSolvingMathExpression;

public sealed partial class TheScoreOfStudentsSolvingMathExpressionReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(TheScoreOfStudentsSolvingMathExpressionReach.IsWithin(new string('a', 19), new int[0]));

    [Fact]
    public void IsWithin_ExpressionLengthPastTheReach_IsFalse() => Assert.False(TheScoreOfStudentsSolvingMathExpressionReach.IsWithin(new string('a', 20), new int[0]));
}
