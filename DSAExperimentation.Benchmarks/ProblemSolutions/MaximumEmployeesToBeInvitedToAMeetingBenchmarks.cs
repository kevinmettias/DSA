using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Connectivity;
using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Employees to Be Invited to a Meeting (LC 2127): favorite[] is a
// functional graph (out-degree exactly 1 everywhere), so ManualPeelAndCycleWalk
// hand-rolls the domain-specific shortcut most submissions use directly against
// int[] favorite - an in-degree peel (Kahn's algorithm, specialized to a single
// successor per node) followed by an explicit forward walk to trace out
// whatever cycles survive the peel - vs. GraphPrimitiveComposition, which
// reaches for this repo's own general-purpose StronglyConnectedComponents.Tarjan
// (a size-k SCC *is* a k-cycle here, since one outgoing edge per node rules out
// any other SCC shape) plus TopologicalSort.TrySort (LargestColorValueInADirectedGraphBenchmarks
// precedent) to get the same off-cycle chain lengths through the general
// IGraphTopology/Dictionary-keyed machinery instead of raw array indices.
// _favorite is built as several mutual (2-cycle) pairs plus one length-5 cycle,
// with every remaining node chaining into an earlier node, so both strategies
// exercise the peel, the multi-cycle walk, and the chain-length DP together
// rather than short-circuiting on an all-cycle or all-chain input.
[MemoryDiagnoser]
public class MaximumEmployeesToBeInvitedToAMeetingBenchmarks
{
    private const int TwoCyclePairCount = 3; // several separate mutual pairs, so bonuses sum across pairs
    private const int LongCycleLength = 5; // one cycle length >= 3, to exercise the "longest cycle" branch
    private const int MutualPairCycleLength = 2; // a 2-cycle is a mutual-favorite pair, scored differently from longer cycles

    [Params(200, 5_000)]
    public int NodeCount;

    private int[] _favorite = null!;

    [GlobalSetup]
    public void Setup() => _favorite = BuildFavorites(NodeCount);

    [Benchmark(Baseline = true)]
    public int ManualPeelAndCycleWalk()
    {
        var n = _favorite.Length;
        var state = new PeelState(n, _favorite);
        var queue = new Queue<int>();

        for (var i = 0; i < n; i++)
        {
            if (state.InDegree[i] == 0)
            {
                queue.Enqueue(i);
            }
        }

        while (queue.Count > 0)
        {
            PeelOne(queue, state);
        }

        return WalkCycles(_favorite, state.Peeled, state.ChainLength);
    }

    private void PeelOne(Queue<int> queue, PeelState state)
    {
        var u = queue.Dequeue();
        state.Peeled[u] = true;
        var f = _favorite[u];
        state.ChainLength[f] = Math.Max(state.ChainLength[f], state.ChainLength[u] + 1);

        if (--state.InDegree[f] == 0)
        {
            queue.Enqueue(f);
        }
    }

    private sealed class PeelState
    {
        public int[] InDegree { get; }

        public bool[] Peeled { get; }

        public int[] ChainLength { get; }

        public PeelState(int n, int[] favorite)
        {
            InDegree = new int[n];
            foreach (var f in favorite)
            {
                InDegree[f]++;
            }

            Peeled = new bool[n];
            ChainLength = new int[n];
        }
    }

    private static int WalkCycles(int[] favorite, bool[] peeled, int[] chainLength)
    {
        var context = new CycleWalkContext(favorite, peeled, chainLength);
        var totals = new CycleTotals();

        for (var start = 0; start < favorite.Length; start++)
        {
            ProcessCycleStart(start, context, totals);
        }

        return Math.Max(totals.LongestCycle, totals.PairedChainsTotal);
    }

    private static void ProcessCycleStart(int start, CycleWalkContext context, CycleTotals totals)
    {
        if (context.Peeled[start] || context.Visited[start])
        {
            return;
        }

        var cycle = TraceCycle(start, context.Favorite, context.Visited);

        if (cycle.Count == MutualPairCycleLength)
        {
            totals.PairedChainsTotal += context.ChainLength[cycle[0]] + context.ChainLength[cycle[1]] + MutualPairCycleLength;
        }
        else
        {
            totals.LongestCycle = Math.Max(totals.LongestCycle, cycle.Count);
        }
    }

    private static List<int> TraceCycle(int start, int[] favorite, bool[] visited)
    {
        var cycle = new List<int>();
        for (var u = start; !visited[u]; u = favorite[u])
        {
            visited[u] = true;
            cycle.Add(u);
        }

        return cycle;
    }

    private sealed class CycleWalkContext
    {
        public int[] Favorite { get; }

        public bool[] Peeled { get; }

        public bool[] Visited { get; }

        public int[] ChainLength { get; }

        public CycleWalkContext(int[] favorite, bool[] peeled, int[] chainLength)
        {
            Favorite = favorite;
            Peeled = peeled;
            ChainLength = chainLength;
            Visited = new bool[favorite.Length];
        }
    }

    private sealed class CycleTotals
    {
        public int LongestCycle { get; set; }

        public int PairedChainsTotal { get; set; }
    }

    [Benchmark]
    public int GraphPrimitiveComposition()
    {
        var nodes = BuildEmployeeGraph(_favorite);

        var components = StronglyConnectedComponents.Tarjan<
            EmployeeNode, EmployeeTopology, ListChildren<EmployeeNode>,
            NaturalChildOrder<EmployeeNode, ListChildren<EmployeeNode>>, ListChildren<EmployeeNode>>(nodes);

        TopologicalSort.TrySort<
            EmployeeNode, EmployeeTopology, ListChildren<EmployeeNode>,
            NaturalChildOrder<EmployeeNode, ListChildren<EmployeeNode>>, ListChildren<EmployeeNode>>(
            nodes, out var ordering);

        var chainLength = ComputeChainLengths(nodes, ordering);

        return ClassifyComponents(components, chainLength);
    }

    private static List<EmployeeNode> BuildEmployeeGraph(int[] favorite)
    {
        var nodes = Enumerable.Range(0, favorite.Length).Select(id => new EmployeeNode(id)).ToList();
        for (var i = 0; i < favorite.Length; i++)
        {
            nodes[i].Successors.Add(nodes[favorite[i]]);
        }

        return nodes;
    }

    private static Dictionary<EmployeeNode, int> ComputeChainLengths(
        List<EmployeeNode> nodes, List<EmployeeNode> ordering)
    {
        var chainLength = nodes.ToDictionary(node => node, _ => 0);
        foreach (var node in ordering)
        {
            var favorite = node.Successors[0];
            chainLength[favorite] = Math.Max(chainLength[favorite], chainLength[node] + 1);
        }

        return chainLength;
    }

    private static int ClassifyComponents(
        List<List<EmployeeNode>> components, Dictionary<EmployeeNode, int> chainLength)
    {
        var longestCycle = 0;
        var pairedChainsTotal = 0;

        foreach (var component in components)
        {
            if (component.Count == MutualPairCycleLength)
            {
                pairedChainsTotal += chainLength[component[0]] + chainLength[component[1]] + MutualPairCycleLength;
            }
            else if (component.Count > MutualPairCycleLength)
            {
                longestCycle = Math.Max(longestCycle, component.Count);
            }
        }

        return Math.Max(longestCycle, pairedChainsTotal);
    }

    private static int[] BuildFavorites(int nodeCount)
    {
        var favorite = new int[nodeCount];
        var idx = AddTwoCyclePairs(favorite);
        idx = AddLongCycle(favorite, idx);
        AddRandomChains(favorite, idx);

        return favorite;
    }

    private static int AddTwoCyclePairs(int[] favorite)
    {
        var idx = 0;
        for (var p = 0; p < TwoCyclePairCount; p++)
        {
            favorite[idx] = idx + 1;
            favorite[idx + 1] = idx;
            idx += MutualPairCycleLength;
        }

        return idx;
    }

    private static int AddLongCycle(int[] favorite, int idx)
    {
        var longCycleStart = idx;
        for (var i = 0; i < LongCycleLength; i++)
        {
            favorite[longCycleStart + i] = longCycleStart + (i + 1) % LongCycleLength;
        }

        return idx + LongCycleLength;
    }

    private static void AddRandomChains(int[] favorite, int idx)
    {
        var random = new Random(1);
        for (var i = idx; i < favorite.Length; i++)
        {
            favorite[i] = random.Next(i); // a uniformly random strictly-earlier node
        }
    }

    // See MaximumEmployeesToBeInvitedToAMeetingTests.Fixtures for the full
    // explanation - repeated here rather than shared because
    // TwoSumBenchmarks/CourseScheduleIIBenchmarks establish this project
    // keeps its own copy of the solution rather than depending on the Tests
    // project.
    private sealed class EmployeeNode(int id)
    {
        public int Id { get; } = id;

        public List<EmployeeNode> Successors { get; } = [];
    }

    private readonly struct EmployeeTopology : IGraphTopology<EmployeeNode, ListChildren<EmployeeNode>>
    {
        public static ListChildren<EmployeeNode> GetChildren(EmployeeNode node) => new(node.Successors);
    }
}
