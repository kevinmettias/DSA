using DigitStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PlusOne;

public sealed partial class PlusOneTests
{
    [Theory]
    [InlineData(new[] { 1, 2, 3 }, new[] { 1, 2, 4 })]
    [InlineData(new[] { 9, 9, 9 }, new[] { 1, 0, 0, 0 })]
    public void PlusOne_LeetCodeExamples_ReturnsIncrementedDigits(int[] digits, int[] expected) => Assert.Equal(expected, PlusOne(digits));
    private static int[] PlusOne(int[] digits) { var stack = new DigitStack(); var carry = 1; for (var i = digits.Length - 1; i >= 0; i--) { var sum = digits[i] + carry; stack.Push(sum % 10); carry = sum / 10; } if (carry > 0) stack.Push(carry); var result = new List<int>(); while (stack.TryPop(out var digit)) result.Add(digit); return result.ToArray(); }
}
