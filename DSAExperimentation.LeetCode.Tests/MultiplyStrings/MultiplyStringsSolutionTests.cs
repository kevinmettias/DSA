using DSAExperimentation.LeetCode.MultiplyStrings;

namespace DSAExperimentation.LeetCode.Tests.MultiplyStrings;

// Harness only. Both strategies live in MultiplyStringsSolution - this file pins
// them to LeetCode's two published examples, three small edge cases, and products
// past what a machine integer holds, up to LeetCode's 200-digit operands. Those
// last rows are what the published examples cannot catch: a strategy that
// converts its operands to a long passes both examples, then throws on two
// 30-digit operands and wraps silently on two 10-digit ones.
public sealed partial class MultiplyStringsSolutionTests
{
    // LeetCode's longest operand.
    private const int MaxDigits = 200;

    public static TheoryData<ProductExample> Examples =>
        new()
        {
            // LeetCode's published examples 1 and 2.
            { new ProductExample(Left: "2", Right: "3", Expected: "6") },
            { new ProductExample(Left: "123", Right: "456", Expected: "56088") },

            // Small edge cases: a zero operand, both operands zero, a carry into a
            // new leading digit.
            { new ProductExample(Left: "0", Right: "12345", Expected: "0") },
            { new ProductExample(Left: "0", Right: "0", Expected: "0") },
            { new ProductExample(Left: "9", Right: "9", Expected: "81") },

            // (10^10 - 1)^2 = 10^20 - 2 * 10^10 + 1: nine 9s, an 8, nine 0s and a 1.
            // Its 20 digits overflow a long, which wraps to 7766279611452241921.
            { new ProductExample(Left: "9999999999", Right: "9999999999", Expected: "99999999980000000001") },

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
                new ProductExample(
                    Left: PowerOfTen(MaxDigits - 1),
                    Right: PowerOfTen(MaxDigits - 1),
                    Expected: PowerOfTen((MaxDigits - 1) * 2))
            },

            // A zero operand against LeetCode's longest one: every place is zero.
            { new ProductExample(Left: "0", Right: new string('9', MaxDigits), Expected: "0") },

            // A one-digit operand against a 200-digit one: 22...2 is 2(10^200 - 1)/9,
            // so five times it is 10(10^200 - 1)/9 - two hundred 1s and a 0.
            { new ProductExample(Left: "5", Right: new string('2', MaxDigits), Expected: new string('1', MaxDigits) + "0") },

            // Two 30-digit operands with no repeating pattern. The product was
            // computed with Python's arbitrary-precision int and agreed with a
            // grade-school multiplication written separately in Python.
            {
                new ProductExample(
                    Left: "123456789012345678901234567890",
                    Right: "987654321098765432109876543210",
                    Expected: "121932631137021795226185032733622923332237463801111263526900")
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MultiplyByColumnSums_LeetCodeExamples_ReturnsDecimalProduct(ProductExample example)
    {
        var actual = MultiplyStringsSolution.MultiplyByColumnSums(example.Left, example.Right);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MultiplyByDigitStack_LeetCodeExamples_ReturnsDecimalProduct(ProductExample example)
    {
        var actual = MultiplyStringsSolution.MultiplyByDigitStack(example.Left, example.Right);

        Assert.Equal(example.Expected, actual);
    }

    // (10^d - 1)^2 = 10^2d - 2 * 10^d + 1, written out: d - 1 nines, an 8, d - 1
    // zeros and a 1.
    private static ProductExample NinesSquared(int digits)
    {
        var nines = new string('9', digits);
        var expected = new string('9', digits - 1) + "8" + new string('0', digits - 1) + "1";

        return new ProductExample(Left: nines, Right: nines, Expected: expected);
    }

    private static string PowerOfTen(int exponent) => "1" + new string('0', exponent);

    // One LeetCode example: the two operands and their product, all three `string`
    // because that is the whole point - neither operand may be converted to a machine
    // integer. A row of three bare string literals does not say which is which, so the
    // fields name the two operands and the answer.
    public readonly record struct ProductExample(string Left, string Right, string Expected);
}
