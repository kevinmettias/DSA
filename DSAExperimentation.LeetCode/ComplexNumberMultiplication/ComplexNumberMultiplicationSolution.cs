namespace DSAExperimentation.LeetCode.ComplexNumberMultiplication;

// LeetCode 537. Complex Number Multiplication: parse "a+bi" into (real, imaginary)
// then apply (a+bi)(c+di) = (ac-bd) + (ad+bc)i. No repo container or algorithm
// primitive applies here - there is nothing to compose over two fixed-size (int,int)
// pairs and one closed-form formula, the same "lighter repo-primitive fit" case as
// Pow(x, n)/Power of Two. The multiplication formula itself is already O(1)
// closed-form arithmetic with no naive-vs-optimal algorithmic split - the real cost
// lives entirely in parsing "a+bi" strings, so the two strategies below differ only
// in parsing: MultiplyByStringSplit is the straightforward string.Split baseline (a
// heap-allocated array plus two substrings per operand); MultiplyBySpanParse instead
// slices with ReadOnlySpan<char> and int.Parse over spans, allocating nothing per
// operand.
internal static class ComplexNumberMultiplicationSolution
{
    // The textbook baseline: string.Split allocates an array plus one substring per
    // component. Deliberately written without this repo's primitives - a complex
    // number here is nothing but a (string, string) pair, LeetCode's own shape.
    public static string MultiplyByStringSplit(string a, string b)
    {
        var (realA, imaginaryA) = ParseSplit(a);
        var (realB, imaginaryB) = ParseSplit(b);

        var real = (realA * realB) - (imaginaryA * imaginaryB);
        var imaginary = (realA * imaginaryB) + (imaginaryA * realB);

        return $"{real}+{imaginary}i";
    }

    private static (int Real, int Imaginary) ParseSplit(string complex)
    {
        var parts = complex[..^1].Split('+');
        return (int.Parse(parts[0]), int.Parse(parts[1]));
    }

    // Slices with ReadOnlySpan<char> and parses directly from the span, allocating
    // nothing per operand.
    public static string MultiplyBySpanParse(string a, string b)
    {
        var (realA, imaginaryA) = ParseSpan(a);
        var (realB, imaginaryB) = ParseSpan(b);

        var real = (realA * realB) - (imaginaryA * imaginaryB);
        var imaginary = (realA * imaginaryB) + (imaginaryA * realB);

        return $"{real}+{imaginary}i";
    }

    private static (int Real, int Imaginary) ParseSpan(ReadOnlySpan<char> complex)
    {
        var separator = complex.IndexOf('+');
        return (int.Parse(complex[..separator]), int.Parse(complex[(separator + 1)..^1]));
    }
}
