using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.Solutions.StepByStepDirectionsFromABinaryTreeNodeToAnother;

// LC 2096's promise about its input: every node has a value of its own, and the start and destination
// values are two different nodes' values. LeetCode's judge never poses an input that breaks it. Asked for
// a value no node holds, the path search answers a partial walk and the ancestor paths throw. Generated
// cases do not always keep the promise, so a harness that runs them asks this first.
internal static class StepByStepDirectionsFromABinaryTreeNodeToAnotherPromise
{
    public static bool IsKeptBy(BinaryTreeNode<int>? root, int startValue, int destValue)
    {
        var values = new List<int>();
        CollectValues(root, values);
        var isEachValueOnce = values.Distinct().Count() == values.Count;
        return isEachValueOnce && startValue != destValue && values.Contains(startValue) && values.Contains(destValue);
    }

    private static void CollectValues(BinaryTreeNode<int>? node, List<int> values)
    {
        if (node is null)
        {
            return;
        }

        values.Add(node.Value);
        CollectValues(node.Left, values);
        CollectValues(node.Right, values);
    }
}
