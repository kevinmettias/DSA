using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DigitOperationsToMakeTwoIntegersEqual;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DigitOperationsToMakeTwoIntegersEqualSolution's,
// the same methods DigitOperationsToMakeTwoIntegersEqualTests proves correct.
// Endpoints are pinned to the widest composite pair at each digit count so
// every rep explores as much of the digit-mutation graph as LC 3377's own
// range allows; the digit graph is built once per DigitCount in
// [GlobalSetup], not charged to the measured Dijkstra call.
[MemoryDiagnoser]
public class DigitOperationsToMakeTwoIntegersEqualBenchmarks
{
    [Params(2, 4)]
    public int DigitCount;

    private int _n;
    private int _m;
    private DigitStepGraph _graph = null!;

    [GlobalSetup]
    public void Setup()
    {
        (_n, _m) = Endpoints(DigitCount);
        _graph = DigitStepGraph.Build(DigitCount);
    }

    private static (int N, int M) Endpoints(int digitCount) => digitCount switch
    {
        2 => (10, 99),
        4 => (1000, 9999),
        _ => throw new ArgumentOutOfRangeException(nameof(digitCount)),
    };

    [Benchmark(Baseline = true)]
    public int BruteForceDijkstra() =>
        DigitOperationsToMakeTwoIntegersEqualSolution.MinOperationsByBruteForceDijkstra(_n, _m);

    [Benchmark]
    public int DijkstraOverDigitGraph() =>
        DigitOperationsToMakeTwoIntegersEqualSolution.MinOperationsByDijkstraOverDigitGraph(_graph, _n, _m);
}
