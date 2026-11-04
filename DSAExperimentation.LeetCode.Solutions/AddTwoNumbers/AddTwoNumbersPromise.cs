using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.Solutions.AddTwoNumbers;

// LC 2's promise about its input: each list writes a number without leading zeros: its last node,
// the most significant digit, is 0 only when the list is the number 0 itself. LeetCode's judge
// never poses an input that breaks it. Converting a list to a number and back drops a leading zero
// the digit walk keeps. Generated cases do not always keep the promise, so a harness that runs them
// asks this first.
internal static class AddTwoNumbersPromise
{
    public static bool IsKeptBy(SinglyLinkedListNode<int>? first, SinglyLinkedListNode<int>? second)
        => HasNoLeadingZero(first) && HasNoLeadingZero(second);

    private static bool HasNoLeadingZero(SinglyLinkedListNode<int>? head)
    {
        var last = head;
        var count = 0;

        for (var node = head; node is not null; node = node.Next)
        {
            last = node;
            count++;
        }

        return last is null || last.Value != 0 || count == 1;
    }
}
