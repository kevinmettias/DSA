using DSAExperimentation.LeetCode.Solutions.KthSymbolInGrammar;

namespace DSAExperimentation.LeetCode.Solutions.Tests.KthSymbolInGrammar;

public sealed partial class KthSymbolInGrammarReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(KthSymbolInGrammarReach.IsWithin(22, 1));

    [Fact]
    public void IsWithin_RowNumberPastTheReach_IsFalse() => Assert.False(KthSymbolInGrammarReach.IsWithin(23, 1));
}
