using System.Runtime.InteropServices;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.NextGreaterNodeInLinkedList;

// LeetCode 1019. Next Greater Node In Linked List: for every node, the value of
// the first strictly larger node that comes after it, or 0 when there is none.
//
// Both strategies start by walking this repo's own SinglyLinkedListNode<int>.Next
// once to materialize the node values (MiddleOfTheLinkedList precedent for the
// node representation), because the answer for a node is only decided by what
// comes later - a list cannot be walked backwards. They differ in how that
// forward question is answered: rescan from every node, or sweep once carrying
// the nodes still waiting for an answer.
internal static class NextGreaterNodeInLinkedListSolution
{
    private const int NoLargerNode = -1;

    // The textbook answer: from every node, scan forward until a larger value
    // turns up. Deliberately BCL only - it is the arm the composed solution below
    // has to justify itself against.
    public static int[] NextLargerNodesByBruteForceScan(SinglyLinkedListNode<int>? head)
    {
        var values = ToValues(head);
        var result = new int[values.Count];

        for (var i = 0; i < values.Count; i++)
        {
            for (var later = i + 1; later < values.Count; later++)
            {
                if (values[later] > values[i])
                {
                    result[i] = values[later];
                    break;
                }
            }
        }

        return result;
    }

    // One NearestBoundary.GreaterToTheRight sweep, the same monotonic decreasing
    // stack NextGreaterElementI/DailyTemperatures already use over an array: each
    // node's nearest strictly larger node after it, found with every index pushed
    // once and popped at most once. The sweep reads ToValues' list in place, since
    // that list is the baseline's too and stays as it is.
    public static int[] NextLargerNodesByMonotonicStackSweep(SinglyLinkedListNode<int>? head)
    {
        var values = ToValues(head);
        var nextLarger = NearestBoundary.GreaterToTheRight(CollectionsMarshal.AsSpan(values), NoLargerNode);
        var result = new int[values.Count];

        for (var i = 0; i < values.Count; i++)
        {
            if (nextLarger[i] != NoLargerNode)
            {
                result[i] = values[nextLarger[i]];
            }
        }

        return result;
    }

    private static List<int> ToValues(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return values;
    }
}
