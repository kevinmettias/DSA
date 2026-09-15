using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountSequencesToK;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountSequencesToKSolution's, the same methods
// CountSequencesToKTests proves correct. Length stays small enough for the
// brute-force arm's 3^Length search to finish; k is fixed at 1, always
// reachable (every "leave unchanged" sequence lands on it), so both arms do
// real search work rather than short-circuiting on an unreachable target.
[MemoryDiagnoser]
public class CountSequencesToKBenchmarks
{
    private const int Seed = 3850;
    private const long Target = 1;

    private int[] _nums = [];

    [Params(8, 12)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = new int[Length];
        for (var i = 0; i < Length; i++)
        {
            _nums[i] = random.Next(1, 7);
        }
    }

    [Benchmark(Baseline = true)]
    public long BruteForceSearch() => CountSequencesToKSolution.CountSequencesByBruteForceSearch(_nums, Target);

    [Benchmark]
    public long PrimeExponentMemo() => CountSequencesToKSolution.CountSequencesByPrimeExponentMemo(_nums, Target);
}
