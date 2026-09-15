using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.JumpGame;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are JumpGameSolution's, the same methods
// JumpGameTests proves correct. _values is built so every jump length
// reaches deep into the rest of the array, forcing the DP baseline through
// its full O(n^2) inner scan instead of short-circuiting on an early
// reachable index.
[MemoryDiagnoser]
public class JumpGameBenchmarks
{
    private const int RandomSeed = 55; // LC problem number
    private const int MinJumpLengthDivisor = 4; private int[] _values = [];

    // keeps every jump length deep into the rest of the array

    [Params(200, 3_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(Length / MinJumpLengthDivisor, Length)).ToArray();
        _values[^1] = 0;
    }

    [Benchmark(Baseline = true)]
    public bool ForwardReachabilityDP() => JumpGameSolution.CanJumpByForwardReachabilityDp(_values);

    [Benchmark]
    public bool GreedyFarthestReach() => JumpGameSolution.CanJumpByGreedyFarthestReach(_values);
}
