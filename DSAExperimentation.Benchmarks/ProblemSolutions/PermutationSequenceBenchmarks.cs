using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PermutationSequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PermutationSequenceSolution's, the same methods
// PermutationSequenceTests proves correct. K is fixed at n! - the lexicographically
// last permutation - so BacktrackEnumeration is always forced through its full
// worst case instead of an early exit on a small k making it look artificially
// competitive.
[MemoryDiagnoser]
public class PermutationSequenceBenchmarks
{
    private int _k;

    [Params(6, 8)]
    public int N { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var factorial = 1;
        for (var i = 1; i <= N; i++)
        {
            factorial *= i;
        }

        _k = factorial;
    }

    [Benchmark(Baseline = true)]
    public string BacktrackEnumeration() => PermutationSequenceSolution.GetPermutationByBacktrackEnumeration(N, _k);

    [Benchmark]
    public string FactoradicSelection() => PermutationSequenceSolution.GetPermutationByFactoradicSelection(N, _k);
}
