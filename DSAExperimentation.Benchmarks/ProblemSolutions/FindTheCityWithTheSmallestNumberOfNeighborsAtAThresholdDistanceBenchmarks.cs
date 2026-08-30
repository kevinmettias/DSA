using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Find the City With the Smallest Number of Neighbors at a Threshold Distance
// (LC 1334): running this repo's own ShortestPath.Dijkstra once per city
// (baseline - the single-source primitive most shortest-path problems in this
// repo reach for, called n times here) vs. a single
// AllPairsShortestPaths.TryComputeDistances call (Floyd-Warshall,
// ShortestPathAlgorithmBenchmarks' own precedent) that computes every pair at
// once - the mirror image of that benchmark's own point about picking the
// wrong tool for an all-pairs question: here Floyd-Warshall is the one
// actually suited to what this problem asks. Roads are wired both directions
// on the shared WeightedGraphNode/WeightedGraphTopology fixtures since LC
// 1334's roads are undirected.
[MemoryDiagnoser]
public class FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistanceBenchmarks
{
    private const int DistanceThreshold = 50;

    [Params(30, 120)]
    public int CityCount;

    private List<WeightedGraphNode> _vertices = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1334);
        _vertices = [.. Enumerable.Range(0, CityCount).Select(id => new WeightedGraphNode(id))];

        for (var i = 1; i < CityCount; i++)
        {
            var j = random.Next(i);
            AddRoad(_vertices[i], _vertices[j], random);
        }

        for (var i = 0; i < CityCount; i++)
        {
            for (var e = 0; e < 2; e++)
            {
                var target = random.Next(CityCount);

                if (target != i)
                {
                    AddRoad(_vertices[i], _vertices[target], random);
                }
            }
        }
    }

    private static void AddRoad(WeightedGraphNode a, WeightedGraphNode b, Random random)
    {
        var weight = random.Next(1, 20);
        a.Edges.Add((weight, b));
        b.Edges.Add((weight, a));
    }

    [Benchmark(Baseline = true)]
    public int DijkstraPerSource()
    {
        var bestCity = -1;
        var bestCount = int.MaxValue;

        for (var city = 0; city < _vertices.Count; city++)
        {
            var source = _vertices[city];
            var distances = ShortestPath.Dijkstra<
                WeightedGraphNode, WeightedGraphTopology, ListEdges<WeightedGraphNode, int>, int>(source);

            var count = distances.Count(pair => pair.Key != source && pair.Value <= DistanceThreshold);

            if (count <= bestCount)
            {
                bestCount = count;
                bestCity = city;
            }
        }

        return bestCity;
    }

    [Benchmark]
    public int FloydWarshallAllPairs()
    {
        AllPairsShortestPaths.TryComputeDistances<
            WeightedGraphNode, WeightedGraphTopology, ListEdges<WeightedGraphNode, int>, int>(
            _vertices, out var distances);

        var bestCity = -1;
        var bestCount = int.MaxValue;

        for (var city = 0; city < _vertices.Count; city++)
        {
            var count = 0;

            for (var other = 0; other < _vertices.Count; other++)
            {
                if (other != city
                    && distances.TryGetValue((_vertices[city], _vertices[other]), out var distance)
                    && distance <= DistanceThreshold)
                {
                    count++;
                }
            }

            if (count <= bestCount)
            {
                bestCount = count;
                bestCity = city;
            }
        }

        return bestCity;
    }
}
