using DSAExperimentation.LeetCode.ComplexNumberMultiplication;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Complex Number Multiplication (LC 537): the multiplication formula itself is
// already O(1) closed-form arithmetic with no naive-vs-optimal algorithmic split -
// the real cost lives entirely in parsing "a+bi" strings, so the comparison is
// between two parsing strategies for the same formula: MultiplyByStringSplit is the
// straightforward string.Split baseline (a heap-allocated array plus two substrings
// per operand); MultiplyBySpanParse instead slices with ReadOnlySpan<char> and
// int.Parse over spans, allocating nothing per operand. Each arm returns every pair's
// product, in pair order.
public class ComplexNumberMultiplicationBenchmarks
{
    private const int RandomSeed = 7;
    private const int MinComponentValue = -100;
    private const int MaxComponentValueExclusive = 101;

    private (string A, string B)[] _pairs = [];

    private string[] _products = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _pairs = new (string, string)[Length];
        _products = new string[Length];

        for (var i = 0; i < Length; i++)
        {
            _pairs[i] = (FormatComplex(random), FormatComplex(random));
        }

        return;

        static string FormatComplex(Random random) => $"{random.Next(MinComponentValue, MaxComponentValueExclusive)}+{random.Next(MinComponentValue, MaxComponentValueExclusive)}i";
    }

    [Benchmark(Baseline = true)]
    public string[] StringSplitParse()
    {
        for (var i = 0; i < _pairs.Length; i++)
        {
            _products[i] = ComplexNumberMultiplicationSolution.MultiplyByStringSplit(_pairs[i].A, _pairs[i].B);
        }

        return _products;
    }

    [Benchmark]
    public string[] SpanParse()
    {
        for (var i = 0; i < _pairs.Length; i++)
        {
            _products[i] = ComplexNumberMultiplicationSolution.MultiplyBySpanParse(_pairs[i].A, _pairs[i].B);
        }

        return _products;
    }
}
