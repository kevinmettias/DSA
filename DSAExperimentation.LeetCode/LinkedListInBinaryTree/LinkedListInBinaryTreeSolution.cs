using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.LinkedListInBinaryTree;

// LeetCode 1367. Linked List in Binary Tree: is there a downward path in the tree
// whose values match the list, in order?
//
// Both strategies share the identical "try every node as a starting point, recurse
// into both children" search - the same shape SubtreeOfAnotherTree uses for whole
// subtree matching. They differ only in how the *remaining* pattern is threaded
// through the recursion: re-slicing a fresh int[1..] at every step (the
// array-shaped habit most first drafts reach for), or walking the
// SinglyLinkedListNode<TValue>.Next chain LeetCode actually hands you.
internal static class LinkedListInBinaryTreeSolution
{
    // The baseline: flatten the list to an array up front, then carry "the rest of
    // the pattern" as an array slice. Deliberately plain BCL - every recursive step
    // allocates a new array, which is exactly the cost the chain walk avoids.
    public static bool IsSubPathByArraySliceWalk(SinglyLinkedListNode<int>? head, BinaryTreeNode<int>? root) =>
        IsSubPathByArraySliceWalk(ToValues(head), root);

    // Prepared-input overload: the benchmark already holds the pattern as an array,
    // so flattening is charged to [GlobalSetup] rather than to the measured search.
    // int[] cannot bind to the SinglyLinkedListNode<int> parameter above, so the two
    // overloads are never ambiguous.
    public static bool IsSubPathByArraySliceWalk(int[] headValues, BinaryTreeNode<int>? root) =>
        root is not null &&
        (HasArrayMatchFromHere(headValues, root) ||
            IsSubPathByArraySliceWalk(headValues, root.Left) ||
            IsSubPathByArraySliceWalk(headValues, root.Right));

    private static bool HasArrayMatchFromHere(int[] headValues, BinaryTreeNode<int>? node) =>
        headValues.Length == 0 ||
        (node is not null && node.Value == headValues[0] &&
            (HasArrayMatchFromHere(headValues[1..], node.Left) || HasArrayMatchFromHere(headValues[1..], node.Right)));

    private static int[] ToValues(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return [.. values];
    }

    // The same search, threading the list itself through the recursion: "the rest of
    // the pattern" is just head.Next, so the walk makes the identical comparisons
    // with zero extra allocation.
    public static bool IsSubPathByLinkedNodeWalk(SinglyLinkedListNode<int>? head, BinaryTreeNode<int>? root) =>
        root is not null &&
        (HasNodeMatchFromHere(head, root) ||
            IsSubPathByLinkedNodeWalk(head, root.Left) ||
            IsSubPathByLinkedNodeWalk(head, root.Right));

    private static bool HasNodeMatchFromHere(SinglyLinkedListNode<int>? head, BinaryTreeNode<int>? node) =>
        head is null ||
        (node is not null && node.Value == head.Value &&
            (HasNodeMatchFromHere(head.Next, node.Left) || HasNodeMatchFromHere(head.Next, node.Right)));
}
