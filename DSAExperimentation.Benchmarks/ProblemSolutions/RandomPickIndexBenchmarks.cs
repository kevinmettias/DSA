using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RandomPickIndex;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RandomPickIndexSolution's, the same classes
// RandomPickIndexTests proves correct. A Design problem's whole point is a
// sequence of calls against one instance, so [GlobalSetup] only prepares the raw
// workload array - not charging that generation to the measured method - and
// each [Benchmark] arm builds its own fresh instance from it (mirroring how a
// real caller constructs a Solution once) before replaying the same PickCalls
// script, returning the running total so the JIT can't eliminate the replay as
// dead code.
[MemoryDiagnoser]
public class RandomPickIndexBenchmarks
{
    private const int Target = 7;
    private const int PickCalls = 500;
    private const int ValueUpperBound = 50;

    private int[] _nums = [];

    [Params(2_000, 50_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(0, ValueUpperBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long ReservoirSampling() => Replay(new RandomPickIndexSolution.RandomPickIndexByReservoirSampling(_nums));

    [Benchmark]
    public long HashMapGrouping() => Replay(new RandomPickIndexSolution.RandomPickIndexByHashMapGrouping(_nums));

    private static long Replay(RandomPickIndexSolution.IRandomPickIndex solution)
    {
        long total = 0;

        for (var call = 0; call < PickCalls; call++)
        {
            total += solution.Pick(Target);
        }

        return total;
    }
}
