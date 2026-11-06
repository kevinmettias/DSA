using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.Solutions.DeleteNodeInABST;

// LC 450 accepts any binary search tree holding the original tree's values less the key ("Another valid answer
// is [5,2,6,null,4,null,7]"), and the tree unchanged when no node holds the key. The catalog records one, so an
// answer stands when its in-order walk is exactly the remaining values, rising. The rebuild arm balances the
// whole tree again, and the tree delete promotes the in-order successor, neither always as the catalog did.
internal static class DeleteNodeInABSTAnswerCheck
{
    public static bool IsAccepted(BinaryTreeNode<int>? root, int key, BinaryTreeNode<int>? answer, BinaryTreeNode<int>? recorded)
    {
        var remaining = LeetCodeSearchTrees.InOrderValues(root).Where(value => value != key).Order();

        return LeetCodeSearchTrees.InOrderValues(answer).SequenceEqual(remaining);
    }
}
