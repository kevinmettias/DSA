using BitStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AddBinary;

public sealed partial class AddBinaryTests
{
    [Theory]
    [InlineData("11", "1", "100")]
    [InlineData("1010", "1011", "10101")]
    public void AddBinary_LeetCodeExamples_ReturnsBinarySum(string a, string b, string expected) => Assert.Equal(expected, Add(a, b));
    private static string Add(string a, string b) { var stack = new BitStack(); var i = a.Length - 1; var j = b.Length - 1; var carry = 0; while (i >= 0 || j >= 0 || carry > 0) { var sum = carry; if (i >= 0) sum += a[i--] - '0'; if (j >= 0) sum += b[j--] - '0'; stack.Push((char)('0' + (sum % 2))); carry = sum / 2; } var chars = new List<char>(); while (stack.TryPop(out var bit)) chars.Add(bit); return new string(chars.ToArray()); }
}
