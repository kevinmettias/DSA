using DSAExperimentation.DataStructures.SinglyLinkedList;
using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NextGreaterNodeInLinkedList;

// LeetCode 1019. Next Greater Node In Linked List: walk this repo's own
// SinglyLinkedListNode<int>.Next once to materialize node values (MiddleOfTheLinkedListTests
// precedent for the node representation), then run the exact same monotonic
// decreasing Stack<int> sweep NextGreaterElementITests/DailyTemperaturesTests already
// use over an array - here the stack holds pending indices, popped and resolved the
// moment a larger value arrives.
public sealed partial class NextGreaterNodeInLinkedListTests
{
    [Fact]
    public void NextLargerNodes_ClassicExampleOne_ReturnsNextGreaterPerNode()
    {
        var result = NextLargerNodes(Build([2, 1, 5]));

        Assert.Equal([5, 5, 0], result);
    }

    [Fact]
    public void NextLargerNodes_ClassicExampleTwo_ReturnsNextGreaterPerNode()
    {
        var result = NextLargerNodes(Build([2, 7, 4, 3, 5]));

        Assert.Equal([7, 0, 5, 5, 0], result);
    }

    [Fact]
    public void NextLargerNodes_StrictlyDecreasing_ReturnsAllZeros()
    {
        var result = NextLargerNodes(Build([9, 7, 5, 3]));

        Assert.Equal([0, 0, 0, 0], result);
    }

    private static int[] NextLargerNodes(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        var result = new int[values.Count];
        var decreasingIndices = new RepoIntStack();

        for (var i = 0; i < values.Count; i++)
        {
            while (decreasingIndices.TryPeek(out var previousIndex) && values[previousIndex] < values[i])
            {
                decreasingIndices.TryPop(out _);
                result[previousIndex] = values[i];
            }

            decreasingIndices.Push(i);
        }

        return result;
    }

    private static SinglyLinkedListNode<int>? Build(int[] values)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;

        foreach (var value in values)
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        return dummy.Next;
    }
}
