using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CircularArrayLoop;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CircularArrayLoopSolution's, the same methods
// CircularArrayLoopTests proves correct. Values are random and nonzero, so a
// genuine cycle is astronomically unlikely to appear and both strategies run
// every starting index to completion instead of exiting early on the first try.
[MemoryDiagnoser]
public class CircularArrayLoopBenchmarks
{
    private const int SignChoiceCount = 2;

    private int[] _values = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _values = Enumerable.Range(0, Length)
            .Select(_ => random.Next(1, Length) * (IsPositiveSign(random) ? 1 : -1))
            .ToArray();
    }

    // The sign is a second independent draw, taken after the magnitude so the two
    // stay in the step order the sequence is seeded from.
    private static bool IsPositiveSign(Random random) => random.Next(SignChoiceCount) == 0;

    [Benchmark(Baseline = true)]
    public bool HasLoopByHashSetWalk() => CircularArrayLoopSolution.HasLoopByHashSetWalk(_values);

    [Benchmark]
    public bool HasLoopByLinkedListFloyd() => CircularArrayLoopSolution.HasLoopByLinkedListFloyd(_values);
}
