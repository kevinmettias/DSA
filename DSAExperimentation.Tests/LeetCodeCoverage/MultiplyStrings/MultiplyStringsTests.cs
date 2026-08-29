namespace DSAExperimentation.Tests.LeetCodeCoverage.MultiplyStrings;

public sealed class MultiplyStringsTests
{
    [Theory]
    [InlineData("2", "3", "6")]
    [InlineData("123", "456", "56088")]
    [InlineData("0", "999", "0")]
    public void Multiply_GradeSchoolDigits_ReturnsDecimalProduct(string num1, string num2, string expected) => Assert.Equal(expected, Multiply(num1, num2));

    private static string Multiply(string num1, string num2)
    {
        if (num1 == "0" || num2 == "0") return "0";
        var digits = new int[num1.Length + num2.Length];
        for (var i = num1.Length - 1; i >= 0; i--)
            for (var j = num2.Length - 1; j >= 0; j--)
            {
                var sum = (num1[i] - '0') * (num2[j] - '0') + digits[i + j + 1];
                digits[i + j + 1] = sum % 10;
                digits[i + j] += sum / 10;
            }
        var start = digits[0] == 0 ? 1 : 0;
        return string.Concat(digits[start..].Select(d => (char)('0' + d)));
    }
}
