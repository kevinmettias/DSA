using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Calculate Amount Paid in Taxes (LC 2303): a single forward pass over the tax
// brackets is already the optimal shape - there is no O(n^2) brute force to fall
// back to here - so this compares a raw array indexer loop (baseline) against the
// same loop over this repo's own ArraySequence<T> (IRandomAccessSequence<T>'s O(1)
// -Get witness), the same "Representation swap should cost nothing" comparison
// DesignBrowserHistoryBenchmarks makes for List<string> vs. DynamicArray<string>.
[MemoryDiagnoser]
public class CalculateAmountPaidInTaxesBenchmarks
{
    [Params(200, 5_000)]
    public int BracketCount;

    private (int Upper, int Percent)[] _brackets = null!;
    private int _income;

    [GlobalSetup]
    public void Setup()
    {
        _brackets = Enumerable.Range(1, BracketCount)
            .Select(i => (Upper: i * 10, Percent: 1 + i % 50))
            .ToArray();
        _income = _brackets[^1].Upper - 1;
    }

    [Benchmark(Baseline = true)]
    public double RawArray()
    {
        var tax = 0.0;
        var previousUpper = 0;

        for (var i = 0; i < _brackets.Length; i++)
        {
            var (upper, percent) = _brackets[i];
            var taxableInBracket = Math.Max(0, Math.Min(_income, upper) - previousUpper);
            tax += taxableInBracket * percent / 100.0;
            previousUpper = upper;

            if (_income <= upper)
            {
                break;
            }
        }

        return tax;
    }

    [Benchmark]
    public double ArraySequence()
    {
        var brackets = new ArraySequence<(int Upper, int Percent)>(_brackets);
        var tax = 0.0;
        var previousUpper = 0;

        for (var i = 0; i < brackets.Length; i++)
        {
            var (upper, percent) = brackets.Get(i);
            var taxableInBracket = Math.Max(0, Math.Min(_income, upper) - previousUpper);
            tax += taxableInBracket * percent / 100.0;
            previousUpper = upper;

            if (_income <= upper)
            {
                break;
            }
        }

        return tax;
    }
}
