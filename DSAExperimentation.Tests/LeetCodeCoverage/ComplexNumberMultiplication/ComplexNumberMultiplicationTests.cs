namespace DSAExperimentation.Tests.LeetCodeCoverage.ComplexNumberMultiplication;

// LeetCode 537. Complex Number Multiplication: parse "a+bi" into (real, imaginary)
// then apply (a+bi)(c+di) = (ac-bd) + (ad+bc)i. No repo container or algorithm
// primitive applies here - there is nothing to compose over two fixed-size (int,int)
// pairs and one closed-form formula, the same "lighter repo-primitive fit" case as
// Pow(x, n)/Power of Two.
public sealed class ComplexNumberMultiplicationTests
{
    [Theory]
    [InlineData("1+1i", "1+1i", "0+2i")]
    [InlineData("1+-1i", "1+-1i", "0+-2i")]
    [InlineData("3+2i", "1+-2i", "7+-4i")]
    public void Multiply_ClassicExamples_ReturnsExpectedProduct(string a, string b, string expected)
        => Assert.Equal(expected, Multiply(a, b));

    private static string Multiply(string a, string b)
    {
        var (realA, imaginaryA) = Parse(a);
        var (realB, imaginaryB) = Parse(b);

        var real = (realA * realB) - (imaginaryA * imaginaryB);
        var imaginary = (realA * imaginaryB) + (imaginaryA * realB);

        return $"{real}+{imaginary}i";
    }

    private static (int Real, int Imaginary) Parse(ReadOnlySpan<char> complex)
    {
        var separator = complex.IndexOf('+');
        var real = int.Parse(complex[..separator]);
        var imaginary = int.Parse(complex[(separator + 1)..^1]);
        return (real, imaginary);
    }
}
