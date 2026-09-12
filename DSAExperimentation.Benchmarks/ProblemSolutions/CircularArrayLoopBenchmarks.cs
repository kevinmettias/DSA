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

    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _values = Enumerable.Range(0, Length)
            .Select(_ => random.Next(1, Length) * (random.Next(SignChoiceCount) == 0 ? 1 : -1))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool HashSetPerStartWalk() => CircularArrayLoopSolution.HasLoopByHashSetWalk(_values);

    [Benchmark]
    public bool LinkedListFloyd() => CircularArrayLoopSolution.HasLoopByLinkedListFloyd(_values);
}
