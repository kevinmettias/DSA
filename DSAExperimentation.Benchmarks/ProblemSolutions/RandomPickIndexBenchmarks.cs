using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.RandomPickIndex;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RandomPickIndexSolution's, the same classes
// RandomPickIndexSolutionTests proves correct. A Design problem's whole point is a
// sequence of calls against one instance, so [GlobalSetup] only prepares the raw
// workload array - not charging that generation to the measured method - and
// each [Benchmark] arm builds its own fresh instance from it (mirroring how a
// real caller constructs a Solution once) before replaying the same PickCalls
// script, returning every picked index in call order. Every instance is built
// with the same PickSeed, so a rebuilt harness replays the same picks; the two
// arms still answer differently, because each consumes the generator its own way.
public class RandomPickIndexBenchmarks
{
    private const int Target = 7;
    private const int PickCalls = 500;
    private const int ValueUpperBound = 50;
    private const int PickSeed = 398; // LC problem number

    private int[] _nums = [];

    private int[] _picks = [];

    [Params(2_000, 50_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _nums = SeededDraws.Values(Length, 0, ValueUpperBound, random);
        _picks = new int[PickCalls];
    }

    [Benchmark(Baseline = true)]
    public int[] ReservoirSampling() =>
        Replay(new RandomPickIndexSolution.RandomPickIndexByReservoirSampling(_nums, PickSeed));

    [Benchmark]
    public int[] HashMapGrouping() =>
        Replay(new RandomPickIndexSolution.RandomPickIndexByHashMapGrouping(_nums, PickSeed));

    private int[] Replay(RandomPickIndexSolution.IRandomPickIndex solution)
    {
        for (var call = 0; call < PickCalls; call++)
        {
            _picks[call] = solution.Pick(Target);
        }

        return _picks;
    }
}
