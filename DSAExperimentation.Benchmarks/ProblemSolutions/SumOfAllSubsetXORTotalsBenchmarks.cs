using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SumOfAllSubsetXORTotals;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SumOfAllSubsetXORTotalsSolution's, the same methods
// SumOfAllSubsetXORTotalsTests proves correct. [GlobalSetup] draws the values so
// generating them is not charged to the measured enumeration; the array itself is
// LeetCode's own input shape, so neither arm needs a hoisted overload.
[MemoryDiagnoser]
public class SumOfAllSubsetXORTotalsBenchmarks
{
    private const int RandomSeed = 1863; // LC problem number
    private const int MaxValueBitWidth = 20; private int[] _values = [];

    // random values are drawn from [1, 2^20)

    [Params(10, 18)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(1, 1 << MaxValueBitWidth)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceBitmask() => SumOfAllSubsetXORTotalsSolution.SubsetXorSumByBitmask(_values);

    [Benchmark]
    public int Backtracking() => SumOfAllSubsetXORTotalsSolution.SubsetXorSumByBacktracking(_values);
}
