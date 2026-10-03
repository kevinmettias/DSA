using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MinimumOperationsToMakeBinaryArrayElementsEqualToOneI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// MinimumOperationsToMakeBinaryArrayElementsEqualToOneISolution's, the same
// methods MinimumOperationsToMakeBinaryArrayElementsEqualToOneISolutionTests proves
// correct. The workload's last 3 elements are pinned to 1 so a trailing zero
// never forces either arm into an early -1 exit.
public class MinimumOperationsToMakeBinaryArrayElementsEqualToOneIBenchmarks
{
    private const int Seed = 3191;

    private int[] _nums = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = SeededDraws.Values(Length, 0, 2, random);
        _nums[^1] = 1;
        _nums[^2] = 1;
        _nums[^3] = 1;
    }

    [Benchmark(Baseline = true)]
    public int ArrayMutation() =>
        MinimumOperationsToMakeBinaryArrayElementsEqualToOneISolution.MinOperationsByArrayMutation(_nums);

    [Benchmark]
    public int FlipParityWindow() =>
        MinimumOperationsToMakeBinaryArrayElementsEqualToOneISolution.MinOperationsByFlipParityWindow(_nums);
}
