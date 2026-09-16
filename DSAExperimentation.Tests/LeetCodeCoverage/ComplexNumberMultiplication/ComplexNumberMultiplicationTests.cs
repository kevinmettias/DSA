using DSAExperimentation.LeetCode.ComplexNumberMultiplication;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ComplexNumberMultiplication;

// LeetCode 537. Complex Number Multiplication: parse "a+bi" into (real, imaginary)
// then apply (a+bi)(c+di) = (ac-bd) + (ad+bc)i. No repo container or algorithm
// primitive applies here - there is nothing to compose over two fixed-size (int,int)
// pairs and one closed-form formula, the same "lighter repo-primitive fit" case as
// Pow(x, n)/Power of Two.
public sealed partial class ComplexNumberMultiplicationTests
{
    public static TheoryData<ComplexProductCase> Examples => new()
    {
        { new ComplexProductCase(Left: "1+1i", Right: "1+1i", Expected: "0+2i") },
        { new ComplexProductCase(Left: "1+-1i", Right: "1+-1i", Expected: "0+-2i") },
        { new ComplexProductCase(Left: "3+2i", Right: "1+-2i", Expected: "7+-4i") },
        { new ComplexProductCase(Left: "-2+3i", Right: "1+-4i", Expected: "10+11i") },
        { new ComplexProductCase(Left: "-3+2i", Right: "4+-5i", Expected: "-2+23i") },
    };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MultiplyByStringSplit_ClassicExamples_ReturnsExpectedProduct(ComplexProductCase example)
    {
        var actual = ComplexNumberMultiplicationSolution.MultiplyByStringSplit(example.Left, example.Right);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MultiplyBySpanParse_ClassicExamples_ReturnsExpectedProduct(ComplexProductCase example)
    {
        var actual = ComplexNumberMultiplicationSolution.MultiplyBySpanParse(example.Left, example.Right);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the two factors and the product they must yield, each
    // spelled "a+bi". All three are strings, so written as three adjacent parameters
    // neither the compiler nor a reader could tell a transposed pair from an intended
    // one; named here, each row says which factor is which. Nested because it is only
    // ever used inside this test class: it is this harness's own vocabulary, not a type
    // another file would import.
    public readonly record struct ComplexProductCase(string Left, string Right, string Expected);
}
