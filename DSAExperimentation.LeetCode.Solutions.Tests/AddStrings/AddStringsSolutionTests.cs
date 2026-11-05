using DSAExperimentation.LeetCode.Solutions.AddStrings;

namespace DSAExperimentation.LeetCode.Solutions.Tests.AddStrings;

// Harness only: both strategies live in AddStringsSolution and are asserted against
// the same cases, among them the carry-out case that grows the result.
public sealed partial class AddStringsSolutionTests
{
    public static TheoryData<DecimalSumCase> Cases =>
        new()
        {
            { new DecimalSumCase(A: "99", B: "1", Expected: "100") },
            { new DecimalSumCase(A: "1", B: "9", Expected: "10") },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void AddByCharArrayReverse_Cases_ReturnsDecimalSum(DecimalSumCase testCase)
    {
        var actual = AddStringsSolution.AddByCharArrayReverse(testCase.A, testCase.B);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void AddByBitStack_Cases_ReturnsDecimalSum(DecimalSumCase testCase)
    {
        var actual = AddStringsSolution.AddByBitStack(testCase.A, testCase.B);

        Assert.Equal(testCase.Expected, actual);
    }

    // One case: the two decimal addends and their sum. All three are `string`
    // and the addition is not symmetric, so the row names what each position is rather
    // than leaving three interchangeable arguments.
    public readonly record struct DecimalSumCase(string A, string B, string Expected);
}
