using DSAExperimentation.LeetCode.MultiplyStrings;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MultiplyStrings;

// Harness only. Both strategies live in MultiplyStringsSolution - this file
// just pins them to LeetCode's published examples plus a couple of edge
// cases. Every example here fits in a long on purpose: MultiplyByLongConversion
// is a baseline representing "what you'd write without this repo", and it
// overflows past ~18-19 digits, exactly why LeetCode's own constraints (up to
// 200 digits) rule that shortcut out for real inputs - MultiplyByDigitStack is
// the strategy this repo relies on beyond that range.
public sealed class MultiplyStringsTests
{
    public static TheoryData<string, string, string> Examples =>
        new()
        {
            { "2", "3", "6" },
            { "123", "456", "56088" },
            { "0", "12345", "0" },
            { "0", "0", "0" },
            { "9", "9", "81" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MultiplyByLongConversion_LeetCodeExamples_ReturnsDecimalProduct(
        string num1, string num2, string expected) =>
        Assert.Equal(expected, MultiplyStringsSolution.MultiplyByLongConversion(num1, num2));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MultiplyByDigitStack_LeetCodeExamples_ReturnsDecimalProduct(
        string num1, string num2, string expected) =>
        Assert.Equal(expected, MultiplyStringsSolution.MultiplyByDigitStack(num1, num2));
}
