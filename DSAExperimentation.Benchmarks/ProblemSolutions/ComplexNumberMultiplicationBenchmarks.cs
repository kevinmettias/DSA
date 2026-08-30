using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Complex Number Multiplication (LC 537): the multiplication formula itself is
// already O(1) closed-form arithmetic with no naive-vs-optimal algorithmic split -
// the real cost lives entirely in parsing "a+bi" strings. No repo container or
// algorithm primitive applies to either parsing strategy below - there is nothing to
// compose over two fixed-size (int,int) pairs and one closed-form formula, the same
// "lighter repo-primitive fit" case as Pow(x, n)/Power of Two - so the comparison is
// between two parsing strategies for the same formula: StringSplitParse is the
// straightforward string.Split baseline (a heap-allocated array plus two substrings
// per operand); SpanParse instead slices with ReadOnlySpan<char> and int.Parse over
// spans, allocating nothing per operand.
[MemoryDiagnoser]
public class ComplexNumberMultiplicationBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private (string A, string B)[] _pairs = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(7);
        _pairs = new (string, string)[Length];

        for (var i = 0; i < Length; i++)
        {
            _pairs[i] = (Format(random), Format(random));
        }

        return;

        static string Format(Random random) => $"{random.Next(-100, 101)}+{random.Next(-100, 101)}i";
    }

    [Benchmark(Baseline = true)]
    public string StringSplitParse()
    {
        var result = string.Empty;

        foreach (var (a, b) in _pairs)
        {
            result = MultiplySplit(a, b);
        }

        return result;
    }

    [Benchmark]
    public string SpanParse()
    {
        var result = string.Empty;

        foreach (var (a, b) in _pairs)
        {
            result = MultiplySpan(a, b);
        }

        return result;
    }

    private static string MultiplySplit(string a, string b)
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

    private static string MultiplySpan(string a, string b)
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
