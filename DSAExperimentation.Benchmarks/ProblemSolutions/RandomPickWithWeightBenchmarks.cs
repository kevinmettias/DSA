using BenchmarkDotNet.Attributes;
using static DSAExperimentation.LeetCode.RandomPickWithWeight.RandomPickWithWeightSolution;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RandomPickWithWeightSolution's, the same classes
// RandomPickWithWeightTests proves correct. A Design problem's whole point is a
// sequence of calls against one instance, so [GlobalSetup] only prepares the raw
// weight workload - not charging that generation to the measured method - and
// each [Benchmark] arm builds its own fresh instance from it (mirroring how a
// real caller constructs a Solution once, and charging the prefix-sum
// construction that instance performs to the same measurement, exactly like
// RandomPickIndexBenchmarks charges its HashMap construction) before replaying
// the same PickCalls script from the same seeded draw sequence, returning the
// running total so the JIT can't eliminate the replay as dead code.
[MemoryDiagnoser]
public class RandomPickWithWeightBenchmarks
{
    private const int PickCalls = 500;
    private const int RandomSeed = 528; // LC problem number
    private const int MaxWeightExclusive = 100;

    [Params(50, 2_000)]
    public int WeightCount;

    private int[] _weights = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _weights = Enumerable.Range(0, WeightCount).Select(_ => random.Next(1, MaxWeightExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long LinearScan() => Replay(new RandomPickWithWeightByLinearScan(_weights, new Random(1)));

    [Benchmark]
    public long BinarySearchUpperBound()
        => Replay(new RandomPickWithWeightByBinarySearchUpperBound(_weights, new Random(1)));

    private static long Replay(IRandomPickWithWeight solution)
    {
        long total = 0;

        for (var call = 0; call < PickCalls; call++)
        {
            total += solution.PickIndex();
        }

        return total;
    }
}
