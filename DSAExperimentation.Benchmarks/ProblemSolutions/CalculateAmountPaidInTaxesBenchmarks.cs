using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.LeetCode.CalculateAmountPaidInTaxes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CalculateAmountPaidInTaxesSolution's, the same methods
// CalculateAmountPaidInTaxesTests proves correct. A single forward pass is already
// the optimal shape here - there is no O(n^2) brute force to fall back to - so this
// is a "Representation swap should cost nothing" comparison, LeetCode's own int[][]
// indexed directly against the same brackets behind IRandomAccessSequence<T>'s Get.
//
// The [upper, percent] pairing is charged to [GlobalSetup] via the sequence arm's
// hoisted overload, so neither arm pays for building its own input. Income sits one
// unit below the top bracket so every arm walks the whole table before breaking.
[MemoryDiagnoser]
public class CalculateAmountPaidInTaxesBenchmarks
{
    private const int UpperStep = 10;
    private const int PercentCycle = 50;

    private int[][] _brackets = [];

    private ArraySequence<(int Upper, int Percent)> _bracketSequence;
    private int _income;
    [Params(200, 5_000)]
    public int BracketCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _brackets = Enumerable.Range(1, BracketCount)
            .Select(i => new[] { i * UpperStep, 1 + i % PercentCycle })
            .ToArray();
        _bracketSequence = CalculateAmountPaidInTaxesSolution.ToBracketSequence(_brackets);
        _income = (BracketCount * UpperStep) - 1;
    }

    [Benchmark(Baseline = true)]
    public double BracketArrayWalk() =>
        CalculateAmountPaidInTaxesSolution.CalculateTaxByBracketArrayWalk(_brackets, _income);

    [Benchmark]
    public double RandomAccessSequenceWalk() =>
        CalculateAmountPaidInTaxesSolution.CalculateTaxByRandomAccessSequence(_bracketSequence, _income);
}
