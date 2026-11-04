using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.Solutions.ConvertBinaryNumberInALinkedListToInteger;

// LC 1290's promise about its input: the list has at most 30 nodes, so the number it writes fits an
// int. LeetCode's judge never poses an input that breaks it. A longer list's number overflows the
// int both strategies answer in. Generated cases do not always keep the promise, so a harness that
// runs them asks this first.
internal static class ConvertBinaryNumberInALinkedListToIntegerPromise
{
    private const int MostNodes = 30;

    public static bool IsKeptBy(SinglyLinkedListNode<int>? head)
    {
        var count = 0;

        for (var node = head; node is not null; node = node.Next)
        {
            count++;
        }

        return count <= MostNodes;
    }
}
