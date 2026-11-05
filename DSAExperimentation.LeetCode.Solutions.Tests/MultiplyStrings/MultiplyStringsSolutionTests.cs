using DSAExperimentation.LeetCode.Solutions.MultiplyStrings;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MultiplyStrings;

// Harness only. Both strategies live in MultiplyStringsSolution - this file pins
// them to products past what a machine integer holds, up to LeetCode's 200-digit
// operands. Those rows are what the published examples cannot catch: a strategy
// that converts its operands to a long passes both examples, then throws on two
// 30-digit operands and wraps silently on two 10-digit ones. LeetCode's published
// examples run in CatalogCaseTests.
public sealed partial class MultiplyStringsSolutionTests
{
    // LeetCode's longest operand.
    private const int MaxDigits = 200;

    public static TheoryData<ProductCase> Cases =>
        new()
        {
            // (10^10 - 1)^2 = 10^20 - 2 * 10^10 + 1: nine 9s, an 8, nine 0s and a 1.
            // Its 20 digits overflow a long, which wraps to 7766279611452241921.
            { new ProductCase(Left: "9999999999", Right: "9999999999", Expected: "99999999980000000001") },

            // The same identity at 30 digits, where long.Parse itself throws:
            // (10^d - 1)^2 is d - 1 nines, an 8, d - 1 zeros and a 1.
            { NinesSquared(digits: 30) },

            // And at LeetCode's longest operands: a 400-digit product, carries in
            // every column.
            { NinesSquared(digits: MaxDigits) },

            // 10^199 * 10^199 = 10^398: a 1 and 398 zeros, 399 digits - one fewer
            // than the 400 places two 200-digit operands can fill, so the
            // product's leading place is a zero that must not be printed.
            {
                new ProductCase(
                    Left: PowerOfTen(MaxDigits - 1),
                    Right: PowerOfTen(MaxDigits - 1),
                    Expected: PowerOfTen((MaxDigits - 1) * 2))
            },

            // A zero operand against LeetCode's longest one: every place is zero.
            { new ProductCase(Left: "0", Right: new string('9', MaxDigits), Expected: "0") },

            // A one-digit operand against a 200-digit one: 22...2 is 2(10^200 - 1)/9,
            // so five times it is 10(10^200 - 1)/9 - two hundred 1s and a 0.
            { new ProductCase(Left: "5", Right: new string('2', MaxDigits), Expected: $"{new string('1', MaxDigits)}0") },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MultiplyByColumnSums_Cases_ReturnsDecimalProduct(ProductCase testCase)
    {
        var actual = MultiplyStringsSolution.MultiplyByColumnSums(testCase.Left, testCase.Right);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MultiplyByDigitStack_Cases_ReturnsDecimalProduct(ProductCase testCase)
    {
        var actual = MultiplyStringsSolution.MultiplyByDigitStack(testCase.Left, testCase.Right);

        Assert.Equal(testCase.Expected, actual);
    }

    // (10^d - 1)^2 = 10^2d - 2 * 10^d + 1, written out: d - 1 nines, an 8, d - 1
    // zeros and a 1.
    private static ProductCase NinesSquared(int digits)
    {
        var nines = new string('9', digits);
        var expected = new string('9', digits - 1) + "8" + new string('0', digits - 1) + "1";

        return new ProductCase(Left: nines, Right: nines, Expected: expected);
    }

    private static string PowerOfTen(int exponent) => "1" + new string('0', exponent);

    // One case: the two operands and their product, all three `string`
    // because that is the whole point - neither operand may be converted to a machine
    // integer. A row of three bare string literals does not say which is which, so the
    // fields name the two operands and the answer.
    public readonly record struct ProductCase(string Left, string Right, string Expected);
}
