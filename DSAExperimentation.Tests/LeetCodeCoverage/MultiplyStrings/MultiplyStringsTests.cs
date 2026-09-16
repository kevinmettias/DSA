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
    public static TheoryData<ProductExample> Examples =>
        new()
        {
            { new ProductExample(Left: "2", Right: "3", Expected: "6") },
            { new ProductExample(Left: "123", Right: "456", Expected: "56088") },
            { new ProductExample(Left: "0", Right: "12345", Expected: "0") },
            { new ProductExample(Left: "0", Right: "0", Expected: "0") },
            { new ProductExample(Left: "9", Right: "9", Expected: "81") },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MultiplyByLongConversion_LeetCodeExamples_ReturnsDecimalProduct(ProductExample example)
    {
        var actual = MultiplyStringsSolution.MultiplyByLongConversion(example.Left, example.Right);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MultiplyByDigitStack_LeetCodeExamples_ReturnsDecimalProduct(ProductExample example)
    {
        var actual = MultiplyStringsSolution.MultiplyByDigitStack(example.Left, example.Right);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the two operands and their product, all three `string`
    // because that is the whole point - neither operand may be converted to a machine
    // integer. A row of three bare string literals does not say which is which, so the
    // fields name the two operands and the answer.
    public readonly record struct ProductExample(string Left, string Right, string Expected);
}
