using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.SumOfAllSubsetXORTotals;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SumOfAllSubsetXORTotalsSolution's, the same methods
// SumOfAllSubsetXORTotalsSolutionTests proves correct. [GlobalSetup] draws the values so
// generating them is not charged to the measured enumeration; the array itself is
// LeetCode's own input shape, so neither arm needs a hoisted overload. Length stops at
// LC 1863's 12 elements, each drawn from its [1, 20].
public class SumOfAllSubsetXORTotalsBenchmarks
{
    private const int RandomSeed = 1863; // LC problem number
    private const int MaxValue = 20;

    private int[] _values = [];

    [Params(10, 12)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = SeededDraws.Values(Length, 1, MaxValue + 1, random);
    }

    [Benchmark(Baseline = true)]
    public int BruteForceBitmask() => SumOfAllSubsetXORTotalsSolution.SubsetXorSumByBitmask(_values);

    [Benchmark]
    public int Backtracking() => SumOfAllSubsetXORTotalsSolution.SubsetXorSumByBacktracking(_values);
}
