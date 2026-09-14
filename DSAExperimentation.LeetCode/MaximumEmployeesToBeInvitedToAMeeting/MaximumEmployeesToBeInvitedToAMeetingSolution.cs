using DSAExperimentation.Algorithms.Connectivity;
using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.MaximumEmployeesToBeInvitedToAMeeting;

// LeetCode 2127. Maximum Employees to Be Invited to a Meeting: every employee
// attends only if seated next to the colleague favorite[i] names, around one round
// table, and the answer is the largest number of employees that can be seated at
// once.
//
// favorite[] is a functional graph - out-degree exactly one everywhere - so every
// walk eventually enters a cycle, and only two seatings are possible: one cycle of
// length >= 3 filling the table by itself, or any number of mutual (2-cycle) pairs
// sharing the table, each pair extended outward by the longest chain of employees
// feeding into its two members. The answer is the larger of those two totals.
//
// Both strategies compute exactly that and differ in what does the cycle-finding
// and the chain-length bookkeeping. ManualPeelAndCycleWalk is the domain-specific
// shortcut most submissions write directly against int[] favorite - an in-degree
// peel (Kahn's algorithm specialized to a single successor per node) followed by
// an explicit forward walk tracing whatever cycles survive the peel.
// GraphPrimitiveComposition reaches for this repo's own general-purpose
// StronglyConnectedComponents.Tarjan - a size-k SCC *is* a k-cycle here, since one
// outgoing edge per node rules out every other SCC shape - plus
// TopologicalSort.TrySort, whose Kahn peel always reports a cycle (returns false)
// while still listing every off-cycle node in dependency order through its
// `ordering` out-parameter, exactly the traversal
// LargestColorValueInADirectedGraphSolution relies on for its own DP.
internal static class MaximumEmployeesToBeInvitedToAMeetingSolution
{
    // A 2-cycle is a mutual-favorite pair, scored differently from longer cycles:
    // several pairs can share one table, a cycle of three or more cannot share with
    // anything.
    private const int MutualPairCycleLength = 2;

    // The textbook answer: peel every node whose in-degree reaches zero, relaxing
    // each peeled node's chain length onto its favorite as it goes, then walk
    // forward from whatever is left to trace the surviving cycles. Deliberately
    // written with BCL arrays and a BCL Queue over raw int indices - it is the arm
    // the composed strategy below has to justify itself against.
    public static int MaximumInvitedByManualPeelAndCycleWalk(int[] favorite)
    {
        var state = new PeelState(favorite);
        var queue = new Queue<int>();

        for (var i = 0; i < favorite.Length; i++)
        {
            if (state.InDegree[i] == 0)
            {
                queue.Enqueue(i);
            }
        }

        while (queue.Count > 0)
        {
            PeelOne(favorite, queue, state);
        }

        return WalkCycles(favorite, state.Peeled, state.ChainLength);
    }

    private static void PeelOne(int[] favorite, Queue<int> queue, PeelState state)
    {
        var u = queue.Dequeue();
        state.Peeled[u] = true;
        var f = favorite[u];
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

        public PeelState(int[] favorite)
        {
            InDegree = new int[favorite.Length];
            foreach (var f in favorite)
            {
                InDegree[f]++;
            }

            Peeled = new bool[favorite.Length];
            ChainLength = new int[favorite.Length];
        }
    }

    // Whatever the peel leaves behind is exactly the union of the cycles, so a
    // forward walk from any unpeeled node re-enters itself and enumerates one
    // cycle.
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
            totals.PairedChainsTotal +=
                context.ChainLength[cycle[0]] + context.ChainLength[cycle[1]] + MutualPairCycleLength;
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

    // LeetCode's own input shape: favorite[i] is the colleague employee i insists
    // on sitting next to.
    public static int MaximumInvitedByGraphPrimitiveComposition(int[] favorite) =>
        MaximumInvitedByGraphPrimitiveComposition(EmployeeGraph.Build(favorite));

    public static int MaximumInvitedByGraphPrimitiveComposition(EmployeeGraph graph)
    {
        var nodes = graph.Nodes;

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

    // Kahn's order guarantees every predecessor of `node` already had its own chain
    // length finalized and relaxed forward before `node` is reached, so one forward
    // pass suffices - cycle nodes never appear in `ordering` (their in-degree never
    // reaches zero), which is exactly what keeps this from walking into a cycle.
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

    // A size-1 component is an off-cycle node and seats nobody on its own; a
    // 2-cycle contributes both members plus their two chains; a longer cycle
    // competes on its own length alone.
    private static int ClassifyComponents(
        List<List<EmployeeNode>> components, Dictionary<EmployeeNode, int> chainLength)
    {
        var longestCycle = 0;
        var pairedChainsTotal = 0;

        foreach (var component in components)
        {
            if (component.Count == MutualPairCycleLength)
            {
                pairedChainsTotal +=
                    chainLength[component[0]] + chainLength[component[1]] + MutualPairCycleLength;
            }
            else if (component.Count > MutualPairCycleLength)
            {
                longestCycle = Math.Max(longestCycle, component.Count);
            }
        }

        return Math.Max(longestCycle, pairedChainsTotal);
    }
}
