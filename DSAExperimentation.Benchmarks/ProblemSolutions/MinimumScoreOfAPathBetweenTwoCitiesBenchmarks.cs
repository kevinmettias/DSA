using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MinimumScoreOfAPathBetweenTwoCities;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumScoreOfAPathBetweenTwoCitiesSolution's, the same
// methods MinimumScoreOfAPathBetweenTwoCitiesSolutionTests proves correct. The road list is
// already LeetCode's own input shape, so [GlobalSetup] hands it over directly and
// neither arm needs a prepared-input overload; each still pays for its own adjacency
// list or disjoint set, which is part of the strategy being measured.
public class MinimumScoreOfAPathBetweenTwoCitiesBenchmarks
{
    private const int RandomSeed = 2492; // LC problem number
    private const int MaxWeightExclusive = 1_000;

    private int[][] _roads = [];

    [Params(200, 5_000)]
    public int CityCount { get; set; }

    // A connected spine 1..CityCount guarantees city 1 and city CityCount share a
    // component, matching this problem's own guarantee, plus extra random chords
    // inside that same spine so more than one candidate minimum weight exists. LC 2492
    // has no repeated roads, so a chord joining two cities some road already joins is
    // dropped.
    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var roads = new List<int[]>();
        var joined = new HashSet<UndirectedEdge>();
        AddSpine(roads, joined, random);
        AddChords(roads, joined, random);
        _roads = roads.ToArray();
    }

    private void AddSpine(List<int[]> roads, HashSet<UndirectedEdge> joined, Random random)
    {
        for (var city = 1; city < CityCount; city++)
        {
            AddUnlessJoined(roads, joined, [city, city + 1, random.Next(1, MaxWeightExclusive)]);
        }
    }

    private void AddChords(List<int[]> roads, HashSet<UndirectedEdge> joined, Random random)
    {
        for (var extra = 0; extra < CityCount; extra++)
        {
            var first = random.Next(1, CityCount + 1);
            var second = random.Next(1, CityCount + 1);

            if (first != second)
            {
                AddUnlessJoined(roads, joined, [first, second, random.Next(1, MaxWeightExclusive)]);
            }
        }
    }

    // The road's weight is drawn before the repeat check, so a dropped chord still consumes
    // its draw and every later road comes out of the seeded stream unchanged.
    private static void AddUnlessJoined(List<int[]> roads, HashSet<UndirectedEdge> joined, int[] road)
    {
        var low = Math.Min(road[0], road[1]);
        var high = Math.Max(road[0], road[1]);

        if (joined.Add(new UndirectedEdge(low, high)))
        {
            roads.Add(road);
        }
    }

    [Benchmark(Baseline = true)]
    public int BreadthFirstFloodFill() =>
        MinimumScoreOfAPathBetweenTwoCitiesSolution.MinScoreByBreadthFirstFloodFill(CityCount, _roads);

    [Benchmark]
    public int DisjointSetUnionFind() =>
        MinimumScoreOfAPathBetweenTwoCitiesSolution.MinScoreByDisjointSet(CityCount, _roads);
}
