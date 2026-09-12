using DSAExperimentation.LeetCode.AddStrings;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AddStrings;

// Harness only: both strategies live in AddStringsSolution and are asserted against
// the same examples, including the carry-out case that grows the result.
public sealed class AddStringsTests
{
    public static TheoryData<string, string, string> Examples =>
        new()
        {
            { "11", "123", "134" },
            { "456", "77", "533" },
            { "0", "0", "0" },
            { "99", "1", "100" },
            { "1", "9", "10" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void AddByCharArrayReverse_LeetCodeExamples_ReturnsDecimalSum(string a, string b, string expected) =>
        Assert.Equal(expected, AddStringsSolution.AddByCharArrayReverse(a, b));

    [Theory]
    [MemberData(nameof(Examples))]
    public void AddByBitStack_LeetCodeExamples_ReturnsDecimalSum(string a, string b, string expected) =>
        Assert.Equal(expected, AddStringsSolution.AddByBitStack(a, b));
}
