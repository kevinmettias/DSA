namespace DSAExperimentation.LeetCode.Solutions.ConstructBinarySearchTreeFromPreorderTraversal;

// LC 1008's promise about its input: the values are unique, and they are the preorder of a binary
// search tree. LeetCode's judge never poses an input that breaks it. Inserting each value in turn
// builds a tree whose preorder is not the input when the input is no search tree's preorder, so the
// two strategies part. Generated cases do not always keep the promise, so a harness that runs them
// asks this first.
internal static class ConstructBinarySearchTreeFromPreorderTraversalPromise
{
    public static bool IsKeptBy(int[] preorder)
        => preorder.Distinct().Count() == preorder.Length && IsSearchTreePreorder(preorder);

    // Each value, once the walk has turned right past an ancestor, must exceed that ancestor.
    private static bool IsSearchTreePreorder(int[] preorder)
    {
        var ancestors = new Stack<int>();
        int? lowerBound = null;

        foreach (var value in preorder)
        {
            if (value < lowerBound)
            {
                return false;
            }

            while (ancestors.Count > 0 && ancestors.Peek() < value)
            {
                lowerBound = ancestors.Pop();
            }

            ancestors.Push(value);
        }

        return true;
    }
}
