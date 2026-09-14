using DSAExperimentation.Algorithms.Traversal.DepthFirst;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.SmallestMissingGeneticValueInEachSubtree;

// LeetCode 2003. Smallest Missing Genetic Value in Each Subtree: parents[] encodes
// a rooted family tree (DataStructures' own ParentArrayTree shape) and nums[] gives
// each node a distinct genetic value in [1, 100000]. For every node, report the
// smallest genetic value from 1 upwards that its subtree does NOT contain - the mex
// of the subtree's value set.
//
// Both strategies answer exactly that; they differ in how many times a node's value
// is looked at.
internal static class SmallestMissingGeneticValueInEachSubtreeSolution
{
    // LeetCode's genetic values start at 1, so 1 is both the smallest possible answer
    // and the value whose location decides every answer that is not 1.
    private const int SmallestGeneticValue = 1;

    // "There is no such node", in all three places this problem needs one: LeetCode's
    // parent array marks the root with it, Array.IndexOf reports a missing value with
    // it, and - because no node ever carries it as an id - the ancestor walk below
    // uses it for "no child has been covered yet".
    private const int NoNode = -1;

    // The textbook answer: for each node independently, walk its whole subtree into a
    // fresh set and count upwards for the first value missing from it. Deliberately
    // written without this repo's primitives - a BCL HashSet and plain recursion -
    // because it is the arm the composed strategy below has to justify itself against.
    // O(n) per node, so O(n^2) on a chain, where every node's subtree is nearly the
    // whole tree.
    public static int[] SmallestMissingValuesBySubtreeRescan(int[] parents, int[] nums) =>
        SmallestMissingValuesBySubtreeRescan(ParentArrayTree.Build(parents), nums);

    public static int[] SmallestMissingValuesBySubtreeRescan(RootedTreeNode[] nodes, int[] nums)
    {
        var answers = new int[nodes.Length];

        for (var i = 0; i < nodes.Length; i++)
        {
            var values = new HashSet<int>();
            CollectSubtreeValues(nodes[i], nums, values);

            var mex = SmallestGeneticValue;
            while (values.Contains(mex))
            {
                mex++;
            }

            answers[i] = mex;
        }

        return answers;
    }

    private static void CollectSubtreeValues(RootedTreeNode node, int[] nums, HashSet<int> values)
    {
        values.Add(nums[node.Id]);

        foreach (var child in node.Children)
        {
            CollectSubtreeValues(child, nums, values);
        }
    }

    // Composed: only the node holding genetic value 1, and its ancestors up to the
    // root, can answer anything other than 1 - every other subtree is missing value 1
    // outright, so its mex is trivially 1. Walking that ancestor chain, each step
    // DFS's only the NEWLY reachable part of the tree (this repo's own
    // DepthFirstSearch.Traverse, skipping the child already covered by the previous
    // step - the same successor-skip technique MaximumGeneticDifferenceQuery uses to
    // process a parent-array family tree exactly once while climbing it), folding
    // every value it meets into a Set<int> and advancing a mex counter that never
    // moves backwards. Each node is visited once and the counter advances at most n
    // times in total, so O(n).
    public static int[] SmallestMissingValuesByAncestorChain(int[] parents, int[] nums) =>
        SmallestMissingValuesByAncestorChain(ParentArrayTree.Build(parents), parents, nums);

    public static int[] SmallestMissingValuesByAncestorChain(RootedTreeNode[] nodes, int[] parents, int[] nums)
    {
        var answers = new int[nodes.Length];
        Array.Fill(answers, SmallestGeneticValue);

        var nodeWithSmallestValue = Array.IndexOf(nums, SmallestGeneticValue);
        if (nodeWithSmallestValue == NoNode)
        {
            return answers;
        }

        var walk = new AncestorWalk(nodes, parents, nums, new Set<int>(), answers);
        var state = new AncestorChainState(nodeWithSmallestValue, Skip: NoNode, Mex: SmallestGeneticValue);

        while (state.Node != NoNode)
        {
            state = AdvanceAncestor(state, walk);
        }

        return answers;
    }

    // Everything the climb carries that does not change from step to step: the tree,
    // the values, the set of values seen so far, and the output buffer. Only the
    // AncestorChainState moves.
    private readonly record struct AncestorWalk(
        RootedTreeNode[] Nodes,
        int[] Parents,
        int[] Nums,
        Set<int> GeneValues,
        int[] Answers);

    // Where the climb is: the node being answered, the child of it already folded in
    // by the previous step (NoNode on the first step), and the running mex.
    private readonly record struct AncestorChainState(int Node, int Skip, int Mex);

    private static AncestorChainState AdvanceAncestor(AncestorChainState state, AncestorWalk walk)
    {
        var skip = state.Skip;
        var newlyVisited = DepthFirstSearch.Traverse(
            walk.Nodes[state.Node], current => current.Children.Where(child => child.Id != skip));

        foreach (var visited in newlyVisited)
        {
            walk.GeneValues.TryAdd(walk.Nums[visited.Id]);
        }

        var mex = state.Mex;
        while (walk.GeneValues.Has(mex))
        {
            mex++;
        }

        walk.Answers[state.Node] = mex;

        return state with { Node = walk.Parents[state.Node], Skip = state.Node, Mex = mex };
    }
}
