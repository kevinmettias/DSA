using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindTheMostCompetitiveSubsequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheMostCompetitiveSubsequenceSolution's, the
// same methods FindTheMostCompetitiveSubsequenceTests proves correct. The workload
// is a seeded random array and a target length of a third of it, so the baseline
// pays (n - k) separate O(n) scans against the composed arm's single sweep -
// RemoveKDigitsBenchmarks' precedent, one removal at a time vs. all of them within
// one pass. Generating the array is [GlobalSetup]'s job, so only the search is
// measured.
[MemoryDiagnoser]
public class FindTheMostCompetitiveSubsequenceBenchmarks
{
    private const int SubsequenceLengthDivisor = 3;
    private const int WorkloadSeed = 1;

    private int[] _nums = [];

    private int _k;
    [Params(500, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(WorkloadSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(0, Length)).ToArray();
        _k = Length / SubsequenceLengthDivisor;
    }

    [Benchmark(Baseline = true)]
    public int[] RepeatedFirstDescentRemoval() =>
        FindTheMostCompetitiveSubsequenceSolution.MostCompetitiveByRepeatedRemoval(_nums, _k);

    [Benchmark]
    public int[] MonotonicStackSweep() =>
        FindTheMostCompetitiveSubsequenceSolution.MostCompetitiveByMonotonicStack(_nums, _k);
}
