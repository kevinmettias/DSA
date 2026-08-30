using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LinkedListInBinaryTree;

// LeetCode 1367. Linked List in Binary Tree: is there a downward root-to-node
// path whose values match head, in order? Same "try every node as a starting
// point, recurse into both children" shape SubtreeOfAnotherTreeTests already
// proves for whole-subtree matching - here the pattern being matched is a
// SinglyLinkedListNode<TValue> chain instead of a second tree.
public sealed partial class LinkedListInBinaryTreeTests
{
    [Fact]
    public void IsSubPath_ListMatchesADownwardPath_ReturnsTrue()
        => Assert.True(IsSubPath(
            List(4, 6, 8),
            new BinaryTreeNode<int>(1)
            {
                Left = new(4) { Left = new(2), Right = new(6) { Right = new(8) } },
                Right = new(5),
            }));

    [Fact]
    public void IsSubPath_ListRunsOutOfMatchingChildren_ReturnsFalse()
        => Assert.False(IsSubPath(
            List(4, 2, 6),
            new BinaryTreeNode<int>(1)
            {
                Left = new(4) { Left = new(2), Right = new(6) { Right = new(8) } },
                Right = new(5),
            }));

    private static bool IsSubPath(SinglyLinkedListNode<int>? head, BinaryTreeNode<int>? root)
        => root is not null && (MatchesFromHere(head, root) || IsSubPath(head, root.Left) || IsSubPath(head, root.Right));

    private static bool MatchesFromHere(SinglyLinkedListNode<int>? head, BinaryTreeNode<int>? node)
        => head is null || (node is not null && node.Value == head.Value &&
            (MatchesFromHere(head.Next, node.Left) || MatchesFromHere(head.Next, node.Right)));

    private static SinglyLinkedListNode<int> List(params int[] values)
    {
        var head = new SinglyLinkedListNode<int>(values[0]);
        var tail = head;

        for (var i = 1; i < values.Length; i++)
        {
            tail.Next = new SinglyLinkedListNode<int>(values[i]);
            tail = tail.Next;
        }

        return head;
    }
}
