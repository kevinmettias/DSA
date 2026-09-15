using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.CompleteBinaryTreeInserter;

// The Insert/Root contract every strategy above implements. Bespoke to this problem:
// no other LeetCode entry shares this shape, so it stays here rather than in
// DataStructures/.
internal interface ICompleteBinaryTreeInserter
{
    BinaryTreeNode<int> Root { get; }

    int Insert(int value);
}
