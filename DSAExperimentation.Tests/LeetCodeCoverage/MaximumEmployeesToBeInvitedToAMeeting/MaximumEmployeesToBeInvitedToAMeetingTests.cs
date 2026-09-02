using DSAExperimentation.Algorithms.Connectivity;
using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.LeetCodeCoverage.MaximumEmployeesToBeInvitedToAMeeting.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumEmployeesToBeInvitedToAMeeting;

// LeetCode 2127. Maximum Employees to Be Invited to a Meeting: favorite[] is a
// functional graph (every node has out-degree exactly 1), so its strongly
// connected components (StronglyConnectedComponents.Tarjan, general
// IGraphTopology - no acyclicity promise to rely on here) are exactly its
// cycles - a size-1 component is an off-cycle node, a size-k component (k >= 2)
// *is* a k-cycle, since one outgoing edge per node rules out any other SCC
// shape. TopologicalSort.TrySort's Kahn peel always reports a cycle (returns
// false) but its `ordering` out-parameter still lists every off-cycle node in
// dependency order for free - exactly the traversal CourseSchedule/
// LargestColorValueInADirectedGraph already rely on for their own DPs - which
// this reuses to relax each node's longest incoming chain onto its favorite
// before that favorite is itself processed. The answer is then the larger of:
// the longest cycle of length >= 3 seated on its own, or the sum, over every
// mutual (2-cycle) pair, of both members' longest chains plus the pair itself -
// multiple 2-cycles can all share one table, a single >=3 cycle cannot share
// with anything else.
public sealed partial class MaximumEmployeesToBeInvitedToAMeetingTests
{
    [Fact]
    public void MaximumInvited_TwoCycleWithChains_ReturnsThree() =>
        Assert.Equal(3, MaximumInvited([2, 2, 1, 2]));

    [Fact]
    public void MaximumInvited_ThreeCycle_ReturnsThree() =>
        Assert.Equal(3, MaximumInvited([1, 2, 0]));

    [Fact]
    public void MaximumInvited_FourCycle_ReturnsFour() =>
        Assert.Equal(4, MaximumInvited([3, 0, 1, 2]));

    private static int MaximumInvited(int[] favorite)
    {
        var nodes = BuildGraph(favorite);

        var components = StronglyConnectedComponents.Tarjan<
            EmployeeNode, EmployeeTopology, ListChildren<EmployeeNode>,
            NaturalChildOrder<EmployeeNode, ListChildren<EmployeeNode>>, ListChildren<EmployeeNode>>(nodes);

        TopologicalSort.TrySort<
            EmployeeNode, EmployeeTopology, ListChildren<EmployeeNode>,
            NaturalChildOrder<EmployeeNode, ListChildren<EmployeeNode>>, ListChildren<EmployeeNode>>(
            nodes, out var ordering);

        var chainLength = ComputeChainLengths(nodes, ordering);

        return ComputeMaxInvited(components, chainLength);
    }

    private static int ComputeMaxInvited(
        List<List<EmployeeNode>> components, Dictionary<EmployeeNode, int> chainLength)
    {
        var longestCycle = 0;
        var pairedChainsTotal = 0;

        foreach (var component in components)
        {
            if (component.Count == 2)
            {
                pairedChainsTotal += chainLength[component[0]] + chainLength[component[1]] + 2;
            }
            else if (component.Count > 2)
            {
                longestCycle = Math.Max(longestCycle, component.Count);
            }
        }

        return Math.Max(longestCycle, pairedChainsTotal);
    }

    // Kahn's order guarantees every predecessor of `node` already had its own
    // chain length finalized and relaxed forward before `node` is dequeued
    // (LargestColorValueInADirectedGraphTests.RelaxNode's same guarantee), so
    // one forward pass over `ordering` suffices - cycle nodes never appear in
    // it (their in-degree never reaches zero), which is exactly what keeps
    // this from walking into a cycle.
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

    private static List<EmployeeNode> BuildGraph(int[] favorite)
    {
        var nodes = Enumerable.Range(0, favorite.Length).Select(id => new EmployeeNode(id)).ToList();

        for (var i = 0; i < favorite.Length; i++)
        {
            nodes[i].Successors.Add(nodes[favorite[i]]);
        }

        return nodes;
    }
}
