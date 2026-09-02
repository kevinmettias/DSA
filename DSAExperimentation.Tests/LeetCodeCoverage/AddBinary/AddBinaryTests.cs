using DSAExperimentation.LeetCode.AddBinary;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AddBinary;

// Harness only: both strategies live in AddBinarySolution and are asserted against
// the same examples, including the carry-out case that grows the result.
public sealed class AddBinaryTests
{
    public static TheoryData<string, string, string> Examples =>
        new()
        {
            { "11", "1", "100" },
            { "1010", "1011", "10101" },
            { "0", "0", "0" },
            { "1111", "1111", "11110" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void AddByCharArrayReverse_LeetCodeExamples_ReturnsBinarySum(string a, string b, string expected) =>
        Assert.Equal(expected, AddBinarySolution.AddByCharArrayReverse(a, b));

    [Theory]
    [MemberData(nameof(Examples))]
    public void AddByBitStack_LeetCodeExamples_ReturnsBinarySum(string a, string b, string expected) =>
        Assert.Equal(expected, AddBinarySolution.AddByBitStack(a, b));
}
