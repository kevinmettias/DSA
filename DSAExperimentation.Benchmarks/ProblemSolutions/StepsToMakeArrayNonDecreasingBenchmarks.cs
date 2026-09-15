using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.StepsToMakeArrayNonDecreasing;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are StepsToMakeArrayNonDecreasingSolution's, the same
// methods StepsToMakeArrayNonDecreasingTests proves correct. Values are a random
// sequence with repeats so removal rounds actually chain instead of finishing
// after one pass, which is what makes the round-simulation baseline pay for its
// extra passes.
[MemoryDiagnoser]
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
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(0, Length)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int SimulateRounds() =>
        StepsToMakeArrayNonDecreasingSolution.TotalStepsBySimulatingRounds(_nums);

    [Benchmark]
    public int MonotonicStackSweep() =>
        StepsToMakeArrayNonDecreasingSolution.TotalStepsByMonotonicStackSweep(_nums);
}
