using DSAExperimentation.LeetCode.Solutions.KthSymbolInGrammar;

namespace DSAExperimentation.LeetCode.Solutions.Tests.KthSymbolInGrammar;

public sealed partial class KthSymbolInGrammarPromiseTests
{
    public static TheoryData<int, int, bool> Inputs =>
        new()
        {
            { 1, 1, true },
            { 5, 16, true },
            { 30, 536_870_912, true },
            { 5, 17, false },
            { 30, 1_073_741_823, false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForAPositionWithinTheRow(int rowNumber, int symbolIndex, bool expected)
        => Assert.Equal(expected, KthSymbolInGrammarPromise.IsKeptBy(rowNumber, symbolIndex));
}
