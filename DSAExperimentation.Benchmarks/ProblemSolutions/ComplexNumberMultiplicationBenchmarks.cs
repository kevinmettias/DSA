using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ComplexNumberMultiplication;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Complex Number Multiplication (LC 537): the multiplication formula itself is
// already O(1) closed-form arithmetic with no naive-vs-optimal algorithmic split -
// the real cost lives entirely in parsing "a+bi" strings, so the comparison is
// between two parsing strategies for the same formula: MultiplyByStringSplit is the
// straightforward string.Split baseline (a heap-allocated array plus two substrings
// per operand); MultiplyBySpanParse instead slices with ReadOnlySpan<char> and
// int.Parse over spans, allocating nothing per operand.
[MemoryDiagnoser]
public class ComplexNumberMultiplicationBenchmarks
{
    private const int RandomSeed = 7;
    private const int MinComponentValue = -100;
    private const int MaxComponentValueExclusive = 101;

    private (string A, string B)[] _pairs = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _pairs = new (string, string)[Length];

        for (var i = 0; i < Length; i++)
        {
            _pairs[i] = (FormatComplex(random), FormatComplex(random));
        }

        return;

        static string FormatComplex(Random random) => $"{random.Next(MinComponentValue, MaxComponentValueExclusive)}+{random.Next(MinComponentValue, MaxComponentValueExclusive)}i";
    }

    [Benchmark(Baseline = true)]
    public string StringSplitParse()
    {
        var result = string.Empty;

        foreach (var (a, b) in _pairs)
        {
            result = ComplexNumberMultiplicationSolution.MultiplyByStringSplit(a, b);
        }

        return result;
    }

    [Benchmark]
    public string SpanParse()
    {
        var result = string.Empty;

        foreach (var (a, b) in _pairs)
        {
            result = ComplexNumberMultiplicationSolution.MultiplyBySpanParse(a, b);
        }

        return result;
    }
}
