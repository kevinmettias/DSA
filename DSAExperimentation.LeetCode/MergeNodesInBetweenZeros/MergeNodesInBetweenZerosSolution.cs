using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.MergeNodesInBetweenZeros;

// LeetCode 2181. Merge Nodes in Between Zeros: the list begins and ends with a 0
// delimiter and never holds two consecutive zeros, so every run of non-zero nodes
// between two delimiters collapses into a single node carrying that run's sum.
//
// Both strategies are O(n) time and return the merged list; what separates them is
// how much they allocate on the way there. The two-pass baseline materializes every
// node's value into a BCL List<int> and buckets that into a second List<int> of
// group sums before building anything, while the single pass walks
// SinglyLinkedListNode<int>.Next once and appends each completed sum straight onto
// the answer - the same "extra pass, extra allocation" vs. "single primitive-native
// pass" contrast MiddleOfTheLinkedListBenchmarks draws for LC 876.
internal static class MergeNodesInBetweenZerosSolution
{
    // The value LeetCode uses to delimit one group of nodes from the next, and the
    // value the list's own first and last nodes carry.
    private const int GroupDelimiter = 0;

    // The textbook answer: copy the values out, bucket them into group sums, and
    // only then build the result list. Deliberately written with nothing but BCL
    // lists - the nodes it hands back are LeetCode's own answer shape, not a
    // primitive this repo lends it - so it is the arm the single pass below has to
    // justify itself against.
    public static SinglyLinkedListNode<int>? MergeNodesByTwoPassValueBuffer(
        SinglyLinkedListNode<int>? head)
    {
        var values = CollectValues(head);
        var groupSums = SumGroups(values);

        return BuildList(groupSums);
    }

    private static List<int> CollectValues(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return values;
    }

    // Starts past the leading delimiter, so the first zero seen closes the first
    // group rather than opening it.
    private static List<int> SumGroups(List<int> values)
    {
        var groupSums = new List<int>();
        var sum = 0;

        for (var i = 1; i < values.Count; i++)
        {
            if (values[i] == GroupDelimiter)
            {
                groupSums.Add(sum);
                sum = 0;
            }
            else
            {
                sum += values[i];
            }
        }

        return groupSums;
    }

    private static SinglyLinkedListNode<int>? BuildList(List<int> values)
    {
        SinglyLinkedListNode<int>? head = null;
        SinglyLinkedListNode<int>? tail = null;

        foreach (var value in values)
        {
            var node = new SinglyLinkedListNode<int>(value);

            if (tail is null)
            {
                head = node;
            }
            else
            {
                tail.Next = node;
            }

            tail = node;
        }

        return head;
    }

    // One left-to-right walk from the node after the leading delimiter, appending
    // each group's sum onto a dummy-headed output list the moment its closing zero
    // arrives - the same dummy-head list-building shape AddTwoNumbersSolution and
    // RemoveZeroSumConsecutiveNodesFromLinkedListSolution use.
    public static SinglyLinkedListNode<int>? MergeNodesBySinglePassSum(
        SinglyLinkedListNode<int>? head)
    {
        var dummy = new SinglyLinkedListNode<int>(GroupDelimiter);
        var tail = dummy;
        var sum = 0;

        for (var node = head?.Next; node is not null; node = node.Next)
        {
            if (node.Value == GroupDelimiter)
            {
                tail.Next = new SinglyLinkedListNode<int>(sum);
                tail = tail.Next;
                sum = 0;
            }
            else
            {
                sum += node.Value;
            }
        }

        return dummy.Next;
    }
}
