using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.Solutions.DeleteNodeInABST;

// LC 450's promise about its input: the tree is a binary search tree whose values are unique. LeetCode's judge
// never poses an input that breaks it. Rebuilt from its values, a tree that repeats a value or holds one out
// of order comes back another tree, so the strategies part from the catalog there. Generated cases do not
// always keep the promise, so a harness that runs them asks this first.
internal static class DeleteNodeInABSTPromise
{
    public static bool IsKeptBy(BinaryTreeNode<int>? root, int key)
    {
        var values = LeetCodeSearchTrees.InOrderValues(root);

        return LeetCodeSearchTrees.IsStrictlyRising(values);
    }
}
