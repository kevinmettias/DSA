using DSAExperimentation.LeetCode.Solutions.ExpressionAddOperators;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ExpressionAddOperators;

public sealed partial class ExpressionAddOperatorsReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(ExpressionAddOperatorsReach.IsWithin("12345678", 0));

    [Fact]
    public void IsWithin_NumberPastTheReach_IsFalse() => Assert.False(ExpressionAddOperatorsReach.IsWithin("123456789", 0));
}
