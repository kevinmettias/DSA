namespace DSAExperimentation.LeetCode.Solutions.KthAncestorOfATreeNode;

// LeetCode's TreeAncestor, the object both strategies make: getKthAncestor answers
// node's kth ancestor, or -1 once the chain to the root runs out first. Bespoke to
// this problem, so it stays here rather than in DataStructures/.
internal interface ITreeAncestor
{
    int GetKthAncestor(int node, int generations);
}
