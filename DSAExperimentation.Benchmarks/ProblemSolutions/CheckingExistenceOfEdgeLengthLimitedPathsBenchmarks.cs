using DSAExperimentation.LeetCode.CheckingExistenceOfEdgeLengthLimitedPaths;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// CheckingExistenceOfEdgeLengthLimitedPathsSolution's, the same methods
// CheckingExistenceOfEdgeLengthLimitedPathsSolutionTests proves correct. The workload is a
// seeded random multigraph with three edges and two queries per node, so the
// baseline pays one full graph walk per query - O(q * (n + e)) - against the
// offline sweep's single pass over both sorted lists, O((e + q) log(e + q) +
// (e + q) * alpha(n)). Generating the edge list and the queries is [GlobalSetup]'s
// job, so only the answering is measured.
public class CheckingExistenceOfEdgeLengthLimitedPathsBenchmarks
{
    private const int RandomSeed = 1697; // LC problem number
    private const int EdgeCountPerNodeMultiplier = 3;
    private const int QueryCountPerNodeMultiplier = 2;
    private const int MaxEdgeWeight = 1_000_000;

    private int[][] _edgeList = [];

    private int[][] _queries = [];
    [Params(100, 2_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var edgeCount = NodeCount * EdgeCountPerNodeMultiplier;
        var queryCount = NodeCount * QueryCountPerNodeMultiplier;

        _edgeList = Enumerable.Range(0, edgeCount).Select(_ => DrawTriple(random)).ToArray();
        _queries = Enumerable.Range(0, queryCount).Select(_ => DrawTriple(random)).ToArray();
    }

    // One [from, to, weight] row, edge or query alike. LC 1697 never joins a node to itself in
    // either, so a second endpoint drawn equal to the first moves on to the next node; the draws
    // themselves stay in order, so every other row is the one the seed always gave.
    private int[] DrawTriple(Random random)
    {
        var from = random.Next(NodeCount);
        var to = random.Next(NodeCount);
        var weight = random.Next(1, MaxEdgeWeight);
        if (to == from)
        {
            to = (to + 1) % NodeCount;
        }

        return [from, to, weight];
    }

    [Benchmark(Baseline = true)]
    public bool[] DfsPerQuery() =>
        CheckingExistenceOfEdgeLengthLimitedPathsSolution.DistanceLimitedPathsExistByPerQueryDfs(
            NodeCount, _edgeList, _queries);

    [Benchmark]
    public bool[] OfflineDisjointSetSweep() =>
        CheckingExistenceOfEdgeLengthLimitedPathsSolution.DistanceLimitedPathsExistByOfflineDisjointSet(
            NodeCount, _edgeList, _queries);
}
