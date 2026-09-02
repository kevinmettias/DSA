using DSAExperimentation.LeetCode.FaultyKeyboard;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FaultyKeyboard;

// Harness only: the algorithms live in FaultyKeyboardSolution. One test method per
// strategy over one shared set of LeetCode's own examples, so a failure names the
// strategy that broke.
public sealed class FaultyKeyboardTests
{
    public static TheoryData<string, string> Examples =>
        new()
        {
            { "string", "rtsng" },
            { "poiinter", "ponter" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FinalStringByReversal_LeetCodeExamples_ReturnsFinalScreenContents(string s, string expected)
        => Assert.Equal(expected, FaultyKeyboardSolution.FinalStringByReversal(s));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FinalStringByDeque_LeetCodeExamples_ReturnsFinalScreenContents(string s, string expected)
        => Assert.Equal(expected, FaultyKeyboardSolution.FinalStringByDeque(s));
}
