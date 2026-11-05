using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.Solutions.CompleteBinaryTreeInserter;

// LeetCode's get_root call, which ICompleteBinaryTreeInserter answers as its Root property (ARCHITECTURE 17.13).
internal static class CompleteBinaryTreeInserterCalls
{
    public static BinaryTreeNode<int> GetRoot(ICompleteBinaryTreeInserter inserter) => inserter.Root;
}
