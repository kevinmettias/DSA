using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MergeInBetweenLinkedLists;

// LeetCode 1669. Merge In Between Linked Lists: pure pointer surgery over this repo's
// mutable SinglyLinkedListNode<T> chain - walk to the node just before index a and the
// node just after index b, then splice list2's head/tail in between. The same
// "traverse-then-rewire-.Next" shape RotateList already uses, just two cut points
// instead of one.
public sealed partial class MergeInBetweenLinkedListsTests
{
    [Fact]
    public void MergeInBetween_LeetCodeExampleOne_SplicesList2InPlaceOfRemovedRange()
    {
        var list1 = BuildList([0, 1, 2, 3, 4, 5]);
        var list2 = BuildList([1000000, 1000001, 1000002]);

        var merged = MergeInBetween(list1, 3, 4, list2);

        Assert.Equal([0, 1, 2, 1000000, 1000001, 1000002, 5], ToArray(merged));
    }

    [Fact]
    public void MergeInBetween_LeetCodeExampleTwo_RemovesLongerRange()
    {
        var list1 = BuildList([0, 1, 2, 3, 4, 5, 6]);
        var list2 = BuildList([1000000, 1000001, 1000002, 1000003, 1000004]);

        var merged = MergeInBetween(list1, 2, 5, list2);

        Assert.Equal([0, 1, 1000000, 1000001, 1000002, 1000003, 1000004, 6], ToArray(merged));
    }

    private static SinglyLinkedListNode<int> MergeInBetween(SinglyLinkedListNode<int> list1, int a, int b, SinglyLinkedListNode<int> list2)
    {
        var before = list1;
        for (var i = 0; i < a - 1; i++) before = before.Next!;

        var after = before;
        for (var i = 0; i < b - a + 2; i++) after = after.Next!;

        before.Next = list2;

        var list2Tail = list2;
        while (list2Tail.Next is not null) list2Tail = list2Tail.Next;
        list2Tail.Next = after;

        return list1;
    }

    private static SinglyLinkedListNode<int> BuildList(int[] values)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;
        foreach (var value in values)
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        return dummy.Next!;
    }

    private static int[] ToArray(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();
        for (var node = head; node is not null; node = node.Next) values.Add(node.Value);
        return values.ToArray();
    }
}
