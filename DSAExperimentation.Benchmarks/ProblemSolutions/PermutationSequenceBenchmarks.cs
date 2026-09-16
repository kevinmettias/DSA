using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PermutationSequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PermutationSequenceSolution's, the same methods
// PermutationSequenceTests proves correct. The rank is fixed at digitCount! - the
// lexicographically last permutation - so BacktrackEnumeration is always forced
// through its full worst case instead of an early exit on a small rank making it look
// artificially competitive.
[MemoryDiagnoser]
public class PermutationSequenceBenchmarks
{
    private int _rank;

    [Params(6, 8)]
    public int DigitCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var factorial = 1;
        for (var i = 1; i <= DigitCount; i++)
        {
            factorial *= i;
        }

        _rank = factorial;
    }

    [Benchmark(Baseline = true)]
    public string BacktrackEnumeration() => PermutationSequenceSolution.GetPermutationByBacktrackEnumeration(DigitCount, _rank);

    [Benchmark]
    public string FactoradicSelection() => PermutationSequenceSolution.GetPermutationByFactoradicSelection(DigitCount, _rank);
}
