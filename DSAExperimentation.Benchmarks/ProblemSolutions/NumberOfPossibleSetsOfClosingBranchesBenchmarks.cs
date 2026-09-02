using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfPossibleSetsOfClosingBranches;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfPossibleSetsOfClosingBranchesSolution's, the
// same methods NumberOfPossibleSetsOfClosingBranchesTests proves agree. Each arm
// gets the prepared input its hoisted overload takes - a distance matrix for the
// hand-rolled Floyd-Warshall, a built BranchNetwork for AllPairsShortestPaths - so
// road-network construction is charged to [GlobalSetup] rather than to the 2^n
// subset search being measured.
//
// LC 2959 caps n at 10 (the subset enumeration is exponential in it), so
// BranchCount stays fixed at the maximum and ExtraEdgesPerNode is the only axis
// that varies - denser graphs shrink shortest paths, changing how many of the
// 1024 subsets pass the maxDistance check without changing the O(2^n * n^3) shape
// either arm does to find out.
[MemoryDiagnoser]
public class NumberOfPossibleSetsOfClosingBranchesBenchmarks
{
    private const int RandomSeed = 2959; // LC problem number
    private const int BranchCount = 10;
    private const int MaxDistance = 20;
    private const int MaxEdgeWeight = 15;

    [Params(1, 3)]
    public int ExtraEdgesPerNode;

    private long[,] _baseDistances = null!;
    private BranchNetwork _network = null!;

    [GlobalSetup]
    public void Setup()
    {
        var roads = BuildRoads(BranchCount, ExtraEdgesPerNode, RandomSeed);
        _baseDistances = NumberOfPossibleSetsOfClosingBranchesSolution.BuildDistanceMatrix(BranchCount, roads);
        _network = BranchNetwork.Build(BranchCount, roads);
    }

    [Benchmark(Baseline = true)]
    public long BruteForceFloydWarshall() =>
        NumberOfPossibleSetsOfClosingBranchesSolution.CountClosingSetsByBruteForceFloydWarshall(_baseDistances, MaxDistance);

    [Benchmark]
    public long AllPairsShortestPaths() =>
        NumberOfPossibleSetsOfClosingBranchesSolution.CountClosingSetsByAllPairsShortestPaths(_network, MaxDistance);

    // Every branch i > 0 gets a "back edge" to some earlier branch (guaranteeing
    // connectivity, the same shape Fixtures.RandomWeightedGraphs uses), plus extra
    // random edges for density.
    private static int[][] BuildRoads(int branchCount, int extraEdgesPerNode, int seed)
    {
        var random = new Random(seed);
        var roads = new List<int[]>();

        for (var i = 1; i < branchCount; i++)
        {
            roads.Add([random.Next(i), i, random.Next(1, MaxEdgeWeight)]);
        }

        for (var i = 0; i < branchCount; i++)
        {
            for (var e = 0; e < extraEdgesPerNode; e++)
            {
                var target = random.Next(branchCount);

                if (target != i)
                {
                    roads.Add([i, target, random.Next(1, MaxEdgeWeight)]);
                }
            }
        }

        return [.. roads];
    }
}
