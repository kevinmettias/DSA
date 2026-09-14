using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ProcessRestrictedFriendRequests;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ProcessRestrictedFriendRequestsSolution's, the same
// methods ProcessRestrictedFriendRequestsTests proves correct - a from-scratch DFS
// reachability baseline (FindIfPathExistsInGraphBenchmarks's own DFS-vs-Union-Find
// precedent) against this repo's own DisjointSet, whose Find is near O(1) amortized
// instead of a fresh O(n + e) walk per request. Both arms are handed LeetCode's own
// input shape, built once in [GlobalSetup].
[MemoryDiagnoser]
public class ProcessRestrictedFriendRequestsBenchmarks
{
    private const int RandomSeed = 2076;

    private const int RestrictionDivisor = 20;

    [Params(300, 3_000)]
    public int NodeCount;

    private int[][] _restrictions = null!;
    private int[][] _requests = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var restrictionCount = Math.Max(1, NodeCount / RestrictionDivisor);

        _restrictions = new int[restrictionCount][];
        for (var i = 0; i < restrictionCount; i++)
        {
            _restrictions[i] = [random.Next(NodeCount), random.Next(NodeCount)];
        }

        _requests = new int[NodeCount][];
        for (var i = 0; i < NodeCount; i++)
        {
            _requests[i] = [random.Next(NodeCount), random.Next(NodeCount)];
        }
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
