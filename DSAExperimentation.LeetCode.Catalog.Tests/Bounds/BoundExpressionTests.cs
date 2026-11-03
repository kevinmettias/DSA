using DSAExperimentation.LeetCode.Catalog.Bounds;

namespace DSAExperimentation.LeetCode.Catalog.Tests.Bounds;

public sealed partial class BoundExpressionTests
{
    public static TheoryData<string, double> Limits =>
        new()
        {
            { "45", 45 },
            { "10^4", 10_000 },
            { "2 * 10^5", 200_000 },
            { "5 x 10^4", 50_000 },
            { "2^31 - 1", 2_147_483_647 },
            { "-2^31", -2_147_483_648 },
            { "10^9 + 7", 1_000_000_007 },
            { "1.5", 1.5 },
            { "10,000", 10_000 },
            { "10^-5", 0.00001 },
        };

    public static TheoryData<string> NotLimits =>
        new()
        {
            "n",
            "nums.length",
            "10^",
            "1,2",
            "",
        };

    [Theory]
    [MemberData(nameof(Limits))]
    public void TryEvaluate_ArithmeticOfPowersAndProducts_ReturnsItsValue(string text, double expected)
    {
        var isLimit = BoundExpression.TryEvaluate(text, out var value);

        Assert.True(isLimit);
        Assert.Equal(expected, value, precision: 9);
    }

    [Theory]
    [MemberData(nameof(NotLimits))]
    public void TryEvaluate_NameOrIncompleteArithmetic_Declines(string text)
    {
        var isLimit = BoundExpression.TryEvaluate(text, out _);

        Assert.False(isLimit);
    }
}
