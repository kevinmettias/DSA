using DSAExperimentation.LeetCode.PrintWordsVertically;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PrintWordsVertically;

// Harness only. Both column builders are PrintWordsVerticallySolution's; this file
// pins them to LeetCode's three published examples plus a single-word sentence,
// which is where a trim that ran one character too far would show up first.
public sealed class PrintWordsVerticallyTests
{
    public static TheoryData<string, string[]> Examples =>
        new()
        {
            { "HOW ARE YOU", ["HAY", "ORO", "WEU"] },
            { "TO BE OR NOT TO BE", ["TBONTB", "OEROOE", "   T"] },
            { "CONTEST IS COMING", ["CIC", "OSO", "N M", "T I", "E N", "S G", "T"] },
            { "ALONE", ["A", "L", "O", "N", "E"] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void PrintVerticallyByListTrimEnd_LeetCodeExamples_ReturnsColumnsWithoutTrailingSpaces(
        string s, string[] expected) =>
        Assert.Equal(expected, PrintWordsVerticallySolution.PrintVerticallyByListTrimEnd(s));

    [Theory]
    [MemberData(nameof(Examples))]
    public void PrintVerticallyByDynamicArrayColumns_LeetCodeExamples_ReturnsColumnsWithoutTrailingSpaces(
        string s, string[] expected) =>
        Assert.Equal(expected, PrintWordsVerticallySolution.PrintVerticallyByDynamicArrayColumns(s));
}
