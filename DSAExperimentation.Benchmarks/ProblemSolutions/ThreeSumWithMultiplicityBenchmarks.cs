using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ThreeSumWithMultiplicity;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ThreeSumWithMultiplicitySolution's, the same methods
// ThreeSumWithMultiplicityTests proves correct - cubic triple-loop counting
// (baseline, the textbook approach) vs. MergeSort plus the sorted two-pointer sweep
// that counts each equal-valued span's combinations directly instead of visiting
// one pair at a time.
[MemoryDiagnoser]
public class ThreeSumWithMultiplicityBenchmarks
{
    private const int Target = 150;
    private const int RandomSeed = 923; // LC 923
    private const int ValueUpperBoundExclusive = 101; // values drawn from [0, 100] per LC 923's constraint

    [Params(80, 300)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        // Values bounded to [0, 100], matching LC 923's own constraint - this small
        // range is what forces genuine multiplicities (repeated values), which is
        // the whole point of this problem versus plain 3Sum.
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(0, ValueUpperBoundExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => ThreeSumWithMultiplicitySolution.CountTripletsByBruteForce(_values, Target);

    [Benchmark]
    public int MergeSortTwoPointers() =>
        ThreeSumWithMultiplicitySolution.CountTripletsByMergeSortTwoPointers(_values, Target);
}
