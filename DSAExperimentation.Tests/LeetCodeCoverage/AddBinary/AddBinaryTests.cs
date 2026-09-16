using DSAExperimentation.LeetCode.AddBinary;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AddBinary;

// Harness only: both strategies live in AddBinarySolution and are asserted against
// the same examples, including the carry-out case that grows the result.
public sealed partial class AddBinaryTests
{
    public static TheoryData<BinarySumExample> Examples =>
        new()
        {
            { new BinarySumExample(A: "11", B: "1", Expected: "100") },
            { new BinarySumExample(A: "1010", B: "1011", Expected: "10101") },
            { new BinarySumExample(A: "0", B: "0", Expected: "0") },
            { new BinarySumExample(A: "1111", B: "1111", Expected: "11110") },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void AddByCharArrayReverse_LeetCodeExamples_ReturnsBinarySum(BinarySumExample example)
    {
        var actual = AddBinarySolution.AddByCharArrayReverse(example.A, example.B);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void AddByBitStack_LeetCodeExamples_ReturnsBinarySum(BinarySumExample example)
    {
        var actual = AddBinarySolution.AddByBitStack(example.A, example.B);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the two binary addends and their sum. All three are `string`
    // and the addition is not symmetric, so the row names what each position is rather
    // than leaving three interchangeable arguments.
    public readonly record struct BinarySumExample(string A, string B, string Expected);
}
