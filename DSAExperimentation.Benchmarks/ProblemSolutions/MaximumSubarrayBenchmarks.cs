using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MaximumSubarray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumSubarraySolution's, the same methods
// MaximumSubarraySolutionTests proves correct.
public class MaximumSubarrayBenchmarks
{
    private const int RandomSeed = 53; // LC problem number
    private const int ValueMagnitude = 50;

    private int[] _values = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = SeededDraws.Values(Length, -ValueMagnitude, ValueMagnitude + 1, random);
    }

    [Benchmark(Baseline = true)]
    public int BruteForceAllSubarrays() => MaximumSubarraySolution.MaxSubArrayByBruteForce(_values);

    [Benchmark]
    public int KadaneSinglePass() => MaximumSubarraySolution.MaxSubArrayByKadaneScan(_values);
}
