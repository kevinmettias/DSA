using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumTimeToTransportAllIndividuals;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumTimeToTransportAllIndividualsSolution's,
// the same methods MinimumTimeToTransportAllIndividualsTests proves correct.
// Each arm is handed the prepared input its hoisted overload takes - plain
// time/mul arrays for the on-the-fly priority-queue walk, a built
// TransportGraph for the Dijkstra walk - so graph construction is charged to
// [GlobalSetup] rather than to the search being measured.
[MemoryDiagnoser]
public class MinimumTimeToTransportAllIndividualsBenchmarks
{
    private const int Seed = 3594;
    private const int Capacity = 3;
    private const int StageCount = 3;

    [Params(6, 10)]
    public int IndividualCount;

    private int[] _time = null!;
    private double[] _mul = null!;
    private TransportGraph _graph = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _time = Enumerable.Range(0, IndividualCount).Select(_ => random.Next(1, 101)).ToArray();
        _mul = Enumerable.Range(0, StageCount).Select(_ => 0.5 + (random.NextDouble() * 1.5)).ToArray();
        _graph = TransportGraph.Build(_time, Capacity, _mul);
    }

    [Benchmark(Baseline = true)]
    public double BruteForceDijkstra() =>
        MinimumTimeToTransportAllIndividualsSolution.MinTimeByBruteForceDijkstra(_time, Capacity, _mul);

    [Benchmark]
    public double DijkstraOverTransportGraph() =>
        MinimumTimeToTransportAllIndividualsSolution.MinTimeByDijkstraOverTransportGraph(_graph);
}
