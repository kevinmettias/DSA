namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

// One firing point - in-order has exactly one visit moment per node (between its left and
// right subtrees), unlike IDepthFirstHooks' Enter/Exit pair.
//
// Visit is an instance member on a struct type parameter, and required: a hook carries what it
// records into - a list, a running count, the answer it is building - as its own fields, and
// InOrderTraversal.Walk returns the hook value it finished with. A default interface member
// reached through a struct type parameter would box the hook, so there is none. A hook that
// holds a reference can be read through it afterwards; one that holds its state by value must
// be read from the value Walk returns.
/// <summary>
/// A hook that <see cref="InOrderTraversal"/> calls once per node, keeping what it records in its own fields.
/// </summary>
public interface IInOrderHooks<TValue>
{
    /// <summary>Acts on one node, after the walk has visited its left subtree and before its right.</summary>
    /// <param name="node">The node being visited.</param>
    /// <param name="depth">The node's depth, counting the root as 0.</param>
    void Visit(BinaryTreeNode<TValue> node, int depth);
}
