using DSAExperimentation.LeetCode.ProcessRestrictedFriendRequests;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ProcessRestrictedFriendRequestsSolution's, the same
// methods ProcessRestrictedFriendRequestsSolutionTests proves correct - a from-scratch DFS
// reachability baseline (FindIfPathExistsInGraphBenchmarks's own DFS-vs-Union-Find
// precedent) against this repo's own DisjointSet, whose Find is near O(1) amortized
// instead of a fresh O(n + e) walk per request. Both arms are handed LeetCode's own
// input shape, built once in [GlobalSetup]. NodeCount stops at LC 2076's 1,000 people,
// which also caps the requests, one per person, at its 1,000.
public class ProcessRestrictedFriendRequestsBenchmarks
{
    private const int RandomSeed = 2076;

    private const int RestrictionDivisor = 20;

    private int[][] _restrictions = [];

    private int[][] _requests = [];
    [Params(300, 1_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var restrictionCount = Math.Max(1, NodeCount / RestrictionDivisor);

        _restrictions = new int[restrictionCount][];
        for (var i = 0; i < restrictionCount; i++)
        {
            _restrictions[i] = DistinctPair(random, NodeCount);
        }

        _requests = new int[NodeCount][];
        for (var i = 0; i < NodeCount; i++)
        {
            _requests[i] = DistinctPair(random, NodeCount);
        }
    }

    // Two different people, as LC 2076's xi != yi and uj != vj require. A second person
    // drawn equal to the first moves on to the next one, so a repeat costs no extra draw.
    private static int[] DistinctPair(Random random, int nodeCount)
    {
        var first = random.Next(nodeCount);
        var second = random.Next(nodeCount);

        if (second == first)
        {
            second = (second + 1) % nodeCount;
        }

        return [first, second];
    }

    [Benchmark(Baseline = true)]
    public bool[] GraphReachabilityCheck() =>
        ProcessRestrictedFriendRequestsSolution.FriendRequestsByReachabilityScan(
            NodeCount, _restrictions, _requests);

    [Benchmark]
    public bool[] DisjointSetUnionFind() =>
        ProcessRestrictedFriendRequestsSolution.FriendRequestsByDisjointSet(
            NodeCount, _restrictions, _requests);
}
