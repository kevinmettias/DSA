using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Process Restricted Friend Requests (LC 2076): a from-scratch DFS reachability
// baseline - for each request, walk the approved-friendship adjacency list to find
// both people's current groups, then check every restriction against those two sets
// (FindIfPathExistsInGraphBenchmarks's own DFS-vs-Union-Find precedent) - vs. this
// repo's own DisjointSet, whose Find is near O(1) amortized instead of a fresh O(n+e)
// walk per request.
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
    public int GraphReachabilityCheck()
    {
        var adjacency = new List<int>[NodeCount];
        for (var i = 0; i < NodeCount; i++)
        {
            adjacency[i] = [];
        }

        var approvedCount = 0;

        foreach (var request in _requests)
        {
            if (TryApproveWithReachability(adjacency, request))
            {
                approvedCount++;
            }
        }

        return approvedCount;
    }

    private bool TryApproveWithReachability(List<int>[] adjacency, int[] request)
    {
        var person = request[0];
        var other = request[1];
        var personGroup = ReachableSet(adjacency, person);

        if (personGroup.Contains(other))
        {
            return true;
        }

        var otherGroup = ReachableSet(adjacency, other);

        if (ViolatesRestriction(personGroup, otherGroup))
        {
            return false;
        }

        adjacency[person].Add(other);
        adjacency[other].Add(person);
        return true;
    }

    private bool ViolatesRestriction(HashSet<int> personGroup, HashSet<int> otherGroup)
    {
        foreach (var restriction in _restrictions)
        {
            var first = restriction[0];
            var second = restriction[1];

            var wouldConnect =
                (personGroup.Contains(first) && otherGroup.Contains(second)) ||
                (personGroup.Contains(second) && otherGroup.Contains(first));

            if (wouldConnect)
            {
                return true;
            }
        }

        return false;
    }

    private static HashSet<int> ReachableSet(List<int>[] adjacency, int start)
    {
        var visited = new HashSet<int> { start };
        var stack = new Stack<int>();
        stack.Push(start);

        while (stack.Count > 0)
        {
            var node = stack.Pop();

            foreach (var next in adjacency[node])
            {
                if (visited.Add(next))
                {
                    stack.Push(next);
                }
            }
        }

        return visited;
    }

    [Benchmark]
    public int DisjointSetUnionFind()
    {
        var friends = new DisjointSet(NodeCount);
        var approvedCount = 0;

        foreach (var request in _requests)
        {
            if (TryApprove(friends, request[0], request[1]))
            {
                approvedCount++;
            }
        }

        return approvedCount;
    }

    private bool TryApprove(DisjointSet friends, int person, int other)
    {
        var personRoot = friends.Find(person);
        var otherRoot = friends.Find(other);

        if (personRoot == otherRoot)
        {
            return true;
        }

        if (WouldViolateRestriction(friends, personRoot, otherRoot))
        {
            return false;
        }

        friends.Union(personRoot, otherRoot);
        return true;
    }

    private bool WouldViolateRestriction(DisjointSet friends, int personRoot, int otherRoot)
    {
        foreach (var restriction in _restrictions)
        {
            var firstRoot = friends.Find(restriction[0]);
            var secondRoot = friends.Find(restriction[1]);

            var wouldConnect =
                (firstRoot == personRoot && secondRoot == otherRoot) ||
                (firstRoot == otherRoot && secondRoot == personRoot);

            if (wouldConnect)
            {
                return true;
            }
        }

        return false;
    }
}
