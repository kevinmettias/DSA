using DSAExperimentation.LeetCode.AddStrings;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AddStrings;

// Harness only: both strategies live in AddStringsSolution and are asserted against
// the same examples, including the carry-out case that grows the result.
public sealed class AddStringsTests
{
    public static TheoryData<DecimalSumExample> Examples =>
        new()
        {
            { new DecimalSumExample(A: "11", B: "123", Expected: "134") },
            { new DecimalSumExample(A: "456", B: "77", Expected: "533") },
            { new DecimalSumExample(A: "0", B: "0", Expected: "0") },
            { new DecimalSumExample(A: "99", B: "1", Expected: "100") },
            { new DecimalSumExample(A: "1", B: "9", Expected: "10") },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void AddByCharArrayReverse_LeetCodeExamples_ReturnsDecimalSum(DecimalSumExample example)
    {
        var actual = AddStringsSolution.AddByCharArrayReverse(example.A, example.B);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void AddByBitStack_LeetCodeExamples_ReturnsDecimalSum(DecimalSumExample example)
    {
        var actual = AddStringsSolution.AddByBitStack(example.A, example.B);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the two decimal addends and their sum. All three are `string`
    // and the addition is not symmetric, so the row names what each position is rather
    // than leaving three interchangeable arguments.
    public readonly record struct DecimalSumExample(string A, string B, string Expected);
}
