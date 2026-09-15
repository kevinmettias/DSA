using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ClosestSubsequenceSum;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ClosestSubsequenceSumSolution's, the same methods
// ClosestSubsequenceSumTests proves correct. The workload is a random array over a
// small magnitude bound and a goal far outside its reachable sum range, so neither
// arm ever lands on an exact match and both scan to completion - which is what makes
// the 2^n baseline and the 2*2^(n/2) meet-in-the-middle arm comparable.
[MemoryDiagnoser]
public class ClosestSubsequenceSumBenchmarks
{
    private const int Goal = 1_000_000;
    private const int RandomSeed = 1755; // LC problem number
    private const int ValueMagnitudeBound = 50;

    private int[] _nums = [];

    [Params(16, 20)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(-ValueMagnitudeBound, ValueMagnitudeBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceAllSubsets() => ClosestSubsequenceSumSolution.MinAbsDifferenceByBruteForceSubsets(_nums, Goal);

    [Benchmark]
    public int MeetInTheMiddle() => ClosestSubsequenceSumSolution.MinAbsDifferenceByMeetInTheMiddle(_nums, Goal);
}
