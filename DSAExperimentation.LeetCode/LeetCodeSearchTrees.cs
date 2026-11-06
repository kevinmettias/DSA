using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode;

// What LeetCode's search-tree questions ask of a tree beyond its shape: its values in order, which rise
// strictly exactly when it is a search tree of unique values. A question that lets any valid search tree
// stand checks an answer this way, and one that promises a search tree checks its input this way, so the
// reading lives once, over the library's in-order walk.
internal static class LeetCodeSearchTrees
{
    public static List<int> InOrderValues(BinaryTreeNode<int>? root)
    {
        var values = new List<int>();
        InOrderTraversal.Walk(root, new CollectHooks(values));

        return values;
    }

    public static bool IsStrictlyRising(IReadOnlyList<int> values)
    {
        var following = values.Skip(1);

        return values.Zip(following).All(pair => pair.First < pair.Second);
    }

    private readonly struct CollectHooks(List<int> values) : IInOrderHooks<int>
    {
        public void Visit(BinaryTreeNode<int> node, int depth) => values.Add(node.Value);
    }
}
