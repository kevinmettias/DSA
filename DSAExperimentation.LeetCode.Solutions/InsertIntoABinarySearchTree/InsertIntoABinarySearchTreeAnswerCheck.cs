using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.Solutions.InsertIntoABinarySearchTree;

// LC 701 accepts any binary search tree holding the original tree's values and the new one ("There may exist
// multiple valid ways for the insertion, as long as the tree remains a BST after insertion. You can return any
// of them"). The catalog records one, the new value hung under the leaf a search for it ends at, so an answer
// stands when its in-order walk is exactly the old values and the new one, rising. The rebuild arm balances
// the whole tree again, and the tree-insert arm keeps the catalog's shape.
internal static class InsertIntoABinarySearchTreeAnswerCheck
{
    public static bool IsAccepted(BinaryTreeNode<int>? root, int newValue, BinaryTreeNode<int>? answer, BinaryTreeNode<int>? recorded)
    {
        var expected = LeetCodeSearchTrees.InOrderValues(root).Append(newValue).Order();

        return LeetCodeSearchTrees.InOrderValues(answer).SequenceEqual(expected);
    }
}
