using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FlowerPlantingWithNoAdjacent;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FlowerPlantingWithNoAdjacentSolution's, the same
// methods FlowerPlantingWithNoAdjacentTests proves correct. The baseline rescans
// the full raw paths array for every garden (O(gardens * paths)) against this
// repo's own ListChildren/IGraphTopology adjacency plus a Set<int> per garden;
// building that adjacency is charged to [GlobalSetup] by handing the composed arm
// the prepared GardenNetwork its hoisted overload takes.
[MemoryDiagnoser]
public class FlowerPlantingWithNoAdjacentBenchmarks
{
    [Params(500, 5_000)]
    public int GardenCount;

    private int[][] _paths = null!;
    private GardenNetwork _network = null!;

    [GlobalSetup]
    public void Setup()
    {
        _paths = Enumerable
            .Range(FlowerPlantingWithNoAdjacentSolution.FirstGarden, GardenCount - 1)
            .Select(garden => new[] { garden, garden + 1 })
            .ToArray();

        _network = GardenNetwork.Build(GardenCount, _paths);
    }

    [Benchmark(Baseline = true)]
    public int[] RawPathsRescan() =>
        FlowerPlantingWithNoAdjacentSolution.GardenNoAdjByRawPathRescan(GardenCount, _paths);

    [Benchmark]
    public int[] AdjacencyListWithSetTracking() =>
        FlowerPlantingWithNoAdjacentSolution.GardenNoAdjByAdjacencyList(_network);
}
