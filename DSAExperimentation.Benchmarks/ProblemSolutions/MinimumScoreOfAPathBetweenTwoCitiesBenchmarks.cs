using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumScoreOfAPathBetweenTwoCities;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumScoreOfAPathBetweenTwoCitiesSolution's, the same
// methods MinimumScoreOfAPathBetweenTwoCitiesTests proves correct. The road list is
// already LeetCode's own input shape, so [GlobalSetup] hands it over directly and
// neither arm needs a prepared-input overload; each still pays for its own adjacency
// list or disjoint set, which is part of the strategy being measured.
[MemoryDiagnoser]
public class MinimumScoreOfAPathBetweenTwoCitiesBenchmarks
{
    private const int RandomSeed = 2492; // LC problem number
    private const int MaxWeightExclusive = 1_000;

    [Params(200, 5_000)]
    public int CityCount;

    private int[][] _roads = null!;

    // A connected spine 1..CityCount guarantees city 1 and city CityCount share a
    // component, matching this problem's own guarantee, plus extra random chords
    // inside that same spine so more than one candidate minimum weight exists.
    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var roads = new List<int[]>();
        AddSpine(roads, random);
        AddChords(roads, random);
        _roads = roads.ToArray();
    }

    private void AddSpine(List<int[]> roads, Random random)
    {
        for (var city = 1; city < CityCount; city++)
        {
            roads.Add([city, city + 1, random.Next(1, MaxWeightExclusive)]);
        }
    }

    private void AddChords(List<int[]> roads, Random random)
    {
        for (var extra = 0; extra < CityCount; extra++)
        {
            var first = random.Next(1, CityCount + 1);
            var second = random.Next(1, CityCount + 1);

            if (first != second)
            {
                roads.Add([first, second, random.Next(1, MaxWeightExclusive)]);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int BreadthFirstFloodFill() =>
        MinimumScoreOfAPathBetweenTwoCitiesSolution.MinScoreByBreadthFirstFloodFill(CityCount, _roads);

    [Benchmark]
    public int DisjointSetUnionFind() =>
        MinimumScoreOfAPathBetweenTwoCitiesSolution.MinScoreByDisjointSet(CityCount, _roads);
}
