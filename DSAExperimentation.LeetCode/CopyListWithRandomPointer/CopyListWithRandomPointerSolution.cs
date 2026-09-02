using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.CopyListWithRandomPointer;

// LeetCode 138. Copy List with Random Pointer: deep-copy a singly linked list
// whose nodes each carry a second Random reference to any node in the list
// (including itself, or null) - the clone's Random must resolve to the CLONE of
// whatever the original pointed at, never the original node itself.
//
// A HashMap<original, clone> memoizes every node the first time it's cloned, so
// recursing into an already-visited node - reachable either through Next or
// through Random, in either order - returns its existing clone instead of
// looping forever or allocating a duplicate.
internal static class CopyListWithRandomPointerSolution
{
    public static RandomLinkedListNode<int>? CopyByHashMapMemo(RandomLinkedListNode<int>? head)
    {
        var clones = new HashMap<RandomLinkedListNode<int>, RandomLinkedListNode<int>>();

        return Clone(head, clones);
    }

    private static RandomLinkedListNode<int>? Clone(
        RandomLinkedListNode<int>? node,
        HashMap<RandomLinkedListNode<int>, RandomLinkedListNode<int>> clones)
    {
        if (node is null)
        {
            return null;
        }

        if (clones.TryGetValue(node, out var existing))
        {
            return existing;
        }

        var clone = new RandomLinkedListNode<int>(node.Value);
        clones.Set(node, clone);
        clone.Next = Clone(node.Next, clones);
        clone.Random = Clone(node.Random, clones);
        return clone;
    }
}
