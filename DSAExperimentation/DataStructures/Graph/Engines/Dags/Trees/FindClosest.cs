using System.Numerics;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

// A free-standing static utility, not a BinarySearchTree<TValue> instance method,
// for a forced reason, not a stylistic one: BinarySearchTree only needs ORDERING
// (IComparable<TValue>/IComparer<TValue>), but "closest by absolute difference"
// needs a METRIC - subtraction and magnitude - which IComparer<T>.Compare's
// sign-only contract cannot express. This repo already has the answer for that
// gap: ShortestPath.cs uses .NET generic math (INumber<TWeight>, IMinMaxValue
// <TWeight>) for exactly this reason. C# forbids re-constraining an outer type's
// own generic parameter inside one of its members, so TValue : INumber<TValue>
// cannot live on a method of BinarySearchTree<TValue> (which is only
// IComparable<TValue>-constrained) without over-constraining the whole class -
// forcing this into its own utility with its own, stricter, method-level
// constraint, over a raw BinaryTreeNode<TValue>? root the same way
// InOrderTraversal operates directly on nodes rather than through a
// BinarySearchTree<TValue> instance.
internal static class FindClosest
{
    // Precondition: root, if non-null, roots a validly BST-ordered tree under
    // TValue's own comparison - unchecked, the same shape as BinarySearch's
    // sortedness law (§9.1). Tie-break: the first strictly-closer value found
    // wins, so an exact tie in distance keeps whichever value the descent reached
    // first (the shallower one) rather than the later, equally-close one.
    public static bool TryFind<TValue>(BinaryTreeNode<TValue>? root, TValue target, out TValue closest)
        where TValue : INumber<TValue>
    {
        if (root is null)
        {
            // presumption: allow -- closest is only meaningful when this returns
            // true, the standard TryGetValue/TryParse out-parameter contract this
            // mirrors.
            closest = default!;
            return false;
        }

        closest = DescendToClosest(root, target);
        return true;
    }

    private static TValue DescendToClosest<TValue>(BinaryTreeNode<TValue> root, TValue target)
        where TValue : INumber<TValue>
    {
        var node = root;
        var best = root.Value;
        var bestDifference = Difference(best, target);

        while (node is not null)
        {
            var difference = Difference(node.Value, target);

            if (difference < bestDifference)
            {
                bestDifference = difference;
                best = node.Value;
            }

            node = NextNode(node, target);
        }

        return best;
    }

    private static TValue Difference<TValue>(TValue value, TValue target)
        where TValue : INumber<TValue>
        => TValue.Abs(value - target);

    private static BinaryTreeNode<TValue>? NextNode<TValue>(BinaryTreeNode<TValue> node, TValue target)
        where TValue : INumber<TValue>
    {
        if (target < node.Value)
        {
            return node.Left;
        }

        return target > node.Value ? node.Right : null;
    }
}
