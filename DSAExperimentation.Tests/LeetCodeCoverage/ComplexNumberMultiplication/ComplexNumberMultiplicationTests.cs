using DSAExperimentation.LeetCode.ComplexNumberMultiplication;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ComplexNumberMultiplication;

// LeetCode 537. Complex Number Multiplication: parse "a+bi" into (real, imaginary)
// then apply (a+bi)(c+di) = (ac-bd) + (ad+bc)i. No repo container or algorithm
// primitive applies here - there is nothing to compose over two fixed-size (int,int)
// pairs and one closed-form formula, the same "lighter repo-primitive fit" case as
// Pow(x, n)/Power of Two.
public sealed class ComplexNumberMultiplicationTests
{
    public static TheoryData<string, string, string> Examples => new()
    {
        { "1+1i", "1+1i", "0+2i" },
        { "1+-1i", "1+-1i", "0+-2i" },
        { "3+2i", "1+-2i", "7+-4i" },
        { "-2+3i", "1+-4i", "10+11i" },
        { "-3+2i", "4+-5i", "-2+23i" },
    };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MultiplyByStringSplit_ClassicExamples_ReturnsExpectedProduct(string a, string b, string expected)
    {
        var actual = ComplexNumberMultiplicationSolution.MultiplyByStringSplit(a, b);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MultiplyBySpanParse_ClassicExamples_ReturnsExpectedProduct(string a, string b, string expected)
    {
        var actual = ComplexNumberMultiplicationSolution.MultiplyBySpanParse(a, b);
        Assert.Equal(expected, actual);
    }
}
