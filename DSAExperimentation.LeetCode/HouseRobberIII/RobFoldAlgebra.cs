using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.HouseRobberIII;

// The (Robbed, NotRobbed) pair for the subtree rooted at each node: Robbed is the
// best sum when this node's house IS taken (so its children must not be), NotRobbed
// is the best sum when it isn't (so each child independently picks its own better
// option). This algebra answers one LeetCode problem and nothing else, which is why
// it lives beside the solution rather than in Algorithms/Folding.
internal readonly struct RobFoldAlgebra : IFoldAlgebra<BinaryTreeNode<int>, (int Robbed, int NotRobbed)>
{
    public static (int Robbed, int NotRobbed) Empty => (0, 0);

    public static (int Robbed, int NotRobbed) Combine(
        BinaryTreeNode<int> node, IReadOnlyList<(int Robbed, int NotRobbed)> children)
    {
        var robbed = node.Value;
        var notRobbed = 0;

        foreach (var child in children)
        {
            robbed += child.NotRobbed;
            notRobbed += Math.Max(child.Robbed, child.NotRobbed);
        }

        return (robbed, notRobbed);
    }
}
