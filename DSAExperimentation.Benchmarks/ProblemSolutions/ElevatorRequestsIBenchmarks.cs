using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ElevatorRequestsI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ElevatorRequestsISolution's - open-coded
// Math.Abs against Algorithms.ShortestPaths' ManhattanHeuristic witness,
// applied purely for its distance computation on a one-column grid rather
// than an actual search. LC 4020 caps n and requests.Length at 100; this
// scales past that ceiling only to confirm neither arm regresses, since both
// are the same O(m) walk.
[MemoryDiagnoser]
public class ElevatorRequestsIBenchmarks
{
    private const int Seed = 4020;

    [Params(100, 5_000)]
    public int RequestCount;

    private int _floorCount;
    private int[] _requests = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _floorCount = RequestCount;
        _requests = Enumerable.Range(0, RequestCount).Select(_ => random.Next(0, _floorCount)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int InlineAbsoluteDifference() =>
        ElevatorRequestsISolution.TotalTimeByInlineAbsoluteDifference(_floorCount, _requests);

    [Benchmark]
    public int ManhattanHeuristicSum() =>
        ElevatorRequestsISolution.TotalTimeByManhattanHeuristic(_floorCount, _requests);
}
