using DSAExperimentation.LeetCode.Solutions.PrintWordsVertically;

namespace DSAExperimentation.LeetCode.Solutions.Tests.PrintWordsVertically;

// Harness only. Both column builders are PrintWordsVerticallySolution's; this file
// pins them to a single-word sentence,
// which is where a trim that ran one character too far would show up first.
public sealed partial class PrintWordsVerticallySolutionTests
{
    public static TheoryData<string, string[]> Cases =>
        new()
        {
            { "ALONE", ["A", "L", "O", "N", "E"] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void PrintVerticallyByListTrimEnd_Cases_ReturnsColumnsWithoutTrailingSpaces(
        string sentence, string[] expected) =>
        Assert.Equal(expected, PrintWordsVerticallySolution.PrintVerticallyByListTrimEnd(sentence));

    [Theory]
    [MemberData(nameof(Cases))]
    public void PrintVerticallyByDynamicArrayColumns_Cases_ReturnsColumnsWithoutTrailingSpaces(
        string sentence, string[] expected) =>
        Assert.Equal(expected, PrintWordsVerticallySolution.PrintVerticallyByDynamicArrayColumns(sentence));
}
