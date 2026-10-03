using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.StepsToMakeArrayNonDecreasing;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are StepsToMakeArrayNonDecreasingSolution's, the same
// methods StepsToMakeArrayNonDecreasingSolutionTests proves correct. Values are a random
// sequence with repeats so removal rounds actually chain instead of finishing
// after one pass, which is what makes the round-simulation baseline pay for its
// extra passes. They are drawn from [1, Length], inside LC 2289's positive values.
public class StepsToMakeArrayNonDecreasingBenchmarks
{
    private const int RandomSeed = 2289; private int[] _nums = [];

    // LC problem number

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = SeededDraws.Values(Length, 1, Length + 1, random);
    }

    [Benchmark(Baseline = true)]
    public int SimulateRounds() =>
        StepsToMakeArrayNonDecreasingSolution.TotalStepsBySimulatingRounds(_nums);

    [Benchmark]
    public int MonotonicStackSweep() =>
        StepsToMakeArrayNonDecreasingSolution.TotalStepsByMonotonicStackSweep(_nums);
}
