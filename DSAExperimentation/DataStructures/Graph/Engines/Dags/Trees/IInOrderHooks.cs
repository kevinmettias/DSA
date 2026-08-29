namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

// A single optional static-virtual firing point, the same shape ILevelGroupedHooks
// uses for its one OnLevel callback - in-order has exactly one visit moment per node
// (between its left and right subtrees), unlike IDepthFirstHooks' Enter/Exit pair.
internal interface IInOrderHooks<TValue>
{
    static virtual void Visit(BinaryTreeNode<TValue> node, int depth)
    {
    }
}
