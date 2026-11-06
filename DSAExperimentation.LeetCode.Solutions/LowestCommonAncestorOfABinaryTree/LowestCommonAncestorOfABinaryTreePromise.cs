using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.Solutions.LowestCommonAncestorOfABinaryTree;

// LC 236's promise about its input: every value in the tree is unique, and p and q are two different values
// the tree holds. LeetCode's judge never poses an input that breaks it. Its judge names p and q by value,
// which picks out one node each only when no value repeats. Generated cases do not always keep the promise,
// so a harness that runs them asks this first.
internal static class LowestCommonAncestorOfABinaryTreePromise
{
    public static bool IsKeptBy(BinaryTreeNode<int> root, int firstValue, int secondValue)
    {
        var values = LeetCodeSearchTrees.InOrderValues(root);
        var isEachValueUnique = values.Distinct().Count() == values.Count;

        return isEachValueUnique && firstValue != secondValue && values.Contains(firstValue) && values.Contains(secondValue);
    }
}
