using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.Solutions.LogicalOrOfTwoBinaryGridsRepresentedAsQuadTrees;

// LC 558 accepts a tree whose inner nodes carry either val ("you can assign the val to True or False when
// isLeaf is False, and both are accepted in the answer"), so an answer stands when it is the recorded tree with
// the inner nodes' values set aside (LeetCodeQuadTrees.IsSameTree). The catalog records its inner nodes as
// false; the strategies keep the true an input's inner node carried, or build one true.
internal static class LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesAnswerCheck
{
    public static bool IsAccepted(QuadTreeNode quadTree1, QuadTreeNode quadTree2, QuadTreeNode answer, QuadTreeNode recorded)
        => LeetCodeQuadTrees.IsSameTree(answer, recorded);
}
