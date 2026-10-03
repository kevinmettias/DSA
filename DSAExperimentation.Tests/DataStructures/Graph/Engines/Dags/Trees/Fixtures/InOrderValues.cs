using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Fixtures;

// The values an in-order walk visits, in visit order - the one question the in-order and BST
// tests ask of a RecordingInOrderHooks log.
internal static class InOrderValues
{
    public static List<int> Of(BinaryTreeNode<int>? root)
    {
        var visited = new List<(int Value, int Depth)>();

        InOrderTraversal.Walk(root, new RecordingInOrderHooks<int>(visited));

        return visited.ConvertAll(v => v.Value);
    }
}
