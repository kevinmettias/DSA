using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.SplitLinkedListInParts;

// LeetCode 725. Split Linked List in Parts: split a singly linked list into k
// consecutive parts as evenly as possible - earlier parts absorb the
// length % k remainder, trailing parts are null once nodes run out.
//
// The two strategies differ only in whether they reuse the input list's own
// nodes or rebuild each part from scratch; both derive per-part sizes from the
// same PartSizing shape.
internal static class SplitLinkedListInPartsSolution
{
    // Baseline: walk the list once to materialize every value into a BCL
    // List<int>, then allocate k brand-new SinglyLinkedListNode<int> chains
    // from slices of it - O(n) extra node allocation on top of the input list.
    // Deliberately written without this repo's in-place pointer trick - it is
    // the arm SplitListToPartsByInPlaceRewire has to justify itself against.
    public static SinglyLinkedListNode<int>?[] SplitListToPartsByArrayRebuild(
        SinglyLinkedListNode<int>? head, int k)
    {
        var values = new List<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        var sizing = new PartSizing(values.Count / k, values.Count % k);
        var parts = new SinglyLinkedListNode<int>?[k];
        var index = 0;

        for (var i = 0; i < k; i++)
        {
            (parts[i], index) = BuildPart(values, sizing, i, index);
        }

        return parts;
    }

    private static (SinglyLinkedListNode<int>? Part, int NextIndex) BuildPart(
        List<int> values, PartSizing sizing, int i, int index)
    {
        var currentSize = sizing.Size + (i < sizing.Extra ? 1 : 0);

        if (currentSize == 0)
        {
            return (null, index);
        }

        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;

        for (var j = 0; j < currentSize; j++)
        {
            tail.Next = new SinglyLinkedListNode<int>(values[index++]);
            tail = tail.Next;
        }

        return (dummy.Next, index);
    }

    // Walks this repo's own SinglyLinkedListNode<int> chain once and cuts its
    // existing Next pointers to carve out each part, reusing every original
    // node - only the k-length result array is new allocation. Mutates the
    // chain reachable from head.
    public static SinglyLinkedListNode<int>?[] SplitListToPartsByInPlaceRewire(
        SinglyLinkedListNode<int>? head, int k)
    {
        var length = 0;

        for (var node = head; node is not null; node = node.Next)
        {
            length++;
        }

        var sizing = new PartSizing(length / k, length % k);
        var parts = new SinglyLinkedListNode<int>?[k];
        var current = head;

        for (var i = 0; i < k && current is not null; i++)
        {
            (parts[i], current) = CarvePart(current, sizing, i);
        }

        return parts;
    }

    private static (SinglyLinkedListNode<int> Part, SinglyLinkedListNode<int>? NextCurrent) CarvePart(
        SinglyLinkedListNode<int> current, PartSizing sizing, int i)
    {
        var last = LastNodeOfPart(current, sizing, i);
        var next = last.Next;
        last.Next = null;

        return (current, next);
    }

    // Walks `first` to the last node of part `i`: the base size, plus one node while
    // the remainder lasts.
    private static SinglyLinkedListNode<int> LastNodeOfPart(
        SinglyLinkedListNode<int> first, PartSizing sizing, int i)
    {
        var last = first;
        var size = sizing.Size + (i < sizing.Extra ? 1 : 0);

        for (var j = 1; j < size; j++)
        {
            last = last.Next!;
        }

        return last;
    }

    // The base part length and how many leading parts get one extra node -
    // shared shape both strategies derive per-part sizes from.
    private readonly record struct PartSizing(int Size, int Extra);
}
