using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Flower Planting With No Adjacent (LC 1042): each garden has at most 3 paths, so
// greedily assigning the first available flower type out of 4 always succeeds. The
// naive baseline rescans the FULL raw paths array for every garden to find its
// already-planted neighbors (O(gardens * paths)) against this repo's own
// ListChildren/IGraphTopology adjacency (built once, O(gardens + paths)) plus a
// Set<int> per garden tracking used flower types instead of a re-scan.
[MemoryDiagnoser]
public class FlowerPlantingWithNoAdjacentBenchmarks
{
    private const int FlowerTypeCount = 4;

    [Params(500, 5_000)]
    public int GardenCount;

    private (int First, int Second)[] _paths = null!;
    private List<GardenNode> _gardens = null!;

    [GlobalSetup]
    public void Setup()
    {
        _paths = Enumerable.Range(0, GardenCount - 1).Select(i => (i, i + 1)).ToArray();

        _gardens = Enumerable.Range(0, GardenCount).Select(id => new GardenNode(id)).ToList();
        foreach (var (first, second) in _paths)
        {
            _gardens[first].ConnectedGardens.Add(_gardens[second]);
            _gardens[second].ConnectedGardens.Add(_gardens[first]);
        }
    }

    [Benchmark(Baseline = true)]
    public int[] RawPathsRescan()
    {
        var flowers = new int[GardenCount];

        for (var garden = 0; garden < GardenCount; garden++)
        {
            var used = ComputeUsedFlowerMask(garden, flowers);
            flowers[garden] = FirstAvailableFlower(used);
        }

        return flowers;
    }

    private int ComputeUsedFlowerMask(int garden, int[] flowers)
    {
        var used = 0;

        foreach (var (first, second) in _paths)
        {
            if (first == garden && flowers[second] != 0)
            {
                used |= 1 << flowers[second];
            }
            else if (second == garden && flowers[first] != 0)
            {
                used |= 1 << flowers[first];
            }
        }

        return used;
    }

    private static int FirstAvailableFlower(int used)
    {
        for (var candidate = 1; candidate <= FlowerTypeCount; candidate++)
        {
            if ((used & (1 << candidate)) == 0)
            {
                return candidate;
            }
        }

        return 0;
    }

    [Benchmark]
    public int[] AdjacencyListWithSetTracking()
    {
        var flowers = new int[GardenCount];

        foreach (var garden in _gardens)
        {
            var usedFlowers = ComputeUsedFlowerSet(garden, flowers);
            flowers[garden.Id] = FirstAvailableFlower(usedFlowers);
        }

        return flowers;
    }

    private static Set<int> ComputeUsedFlowerSet(GardenNode garden, int[] flowers)
    {
        var usedFlowers = new Set<int>();
        var neighbors = GardenTopology.GetChildren(garden);

        for (var i = 0; i < neighbors.Count; i++)
        {
            var neighborFlower = flowers[neighbors.Get(i).Id];
            if (neighborFlower != 0)
            {
                usedFlowers.TryAdd(neighborFlower);
            }
        }

        return usedFlowers;
    }

    private static int FirstAvailableFlower(Set<int> usedFlowers)
    {
        for (var candidate = 1; candidate <= FlowerTypeCount; candidate++)
        {
            if (!usedFlowers.Has(candidate))
            {
                return candidate;
            }
        }

        return 0;
    }

    // See FlowerPlantingWithNoAdjacentTests.Fixtures for the full explanation -
    // repeated here rather than shared because TwoSumBenchmarks/
    // MedianOfTwoSortedArraysBenchmarks establish this project keeps its own copy
    // of the solution rather than depending on the Tests project.
    private sealed class GardenNode(int id)
    {
        public int Id { get; } = id;

        public List<GardenNode> ConnectedGardens { get; } = [];
    }

    private readonly struct GardenTopology : IGraphTopology<GardenNode, ListChildren<GardenNode>>
    {
        public static ListChildren<GardenNode> GetChildren(GardenNode node) => new(node.ConnectedGardens);
    }
}
