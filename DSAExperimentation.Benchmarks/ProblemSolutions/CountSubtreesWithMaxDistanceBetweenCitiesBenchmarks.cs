using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountSubtreesWithMaxDistanceBetweenCities;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountSubtreesWithMaxDistanceBetweenCitiesSolution's,
// the same methods CountSubtreesWithMaxDistanceBetweenCitiesTests proves correct.
// Each arm is handed the prepared adjacency list its hoisted overload takes, so
// tree construction is charged to [GlobalSetup] rather than to the mask sweep being
// measured.
[MemoryDiagnoser]
public class CountSubtreesWithMaxDistanceBetweenCitiesBenchmarks
{
    // LC problem number, reused as the deterministic tree seed.
    private const int RandomSeed = 1617;

    private List<int>[] _adjacency = [];

    [Params(10, 14)]
    public int CityCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _adjacency = new List<int>[CityCount];

        for (var i = 0; i < CityCount; i++)
        {
            _adjacency[i] = [];
        }

        // A random recursive tree: each city after the first attaches to a
        // uniformly-chosen earlier city, giving a connected, cycle-free graph on
        // CityCount cities with CityCount - 1 edges.
        for (var i = 1; i < CityCount; i++)
        {
            var parent = random.Next(i);
            _adjacency[i].Add(parent);
            _adjacency[parent].Add(i);
        }
    }

    [Benchmark(Baseline = true)]
    public int[] AllPairsBfsPerMask() =>
        CountSubtreesWithMaxDistanceBetweenCitiesSolution.CountSubtreesByAllPairsBfs(_adjacency);

    [Benchmark]
    public int[] DoubleBfsPerMask() =>
        CountSubtreesWithMaxDistanceBetweenCitiesSolution.CountSubtreesByDoubleBfs(_adjacency);
}
