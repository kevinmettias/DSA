using DSAExperimentation.Tests.LeetCodeCoverage.FlattenAMultilevelDoublyLinkedList.Fixtures;
using PendingStack = DSAExperimentation.DataStructures.Stack.Stack<DSAExperimentation.Tests.LeetCodeCoverage.FlattenAMultilevelDoublyLinkedList.Fixtures.Node>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FlattenAMultilevelDoublyLinkedList;

// LeetCode 430. Flatten a Multilevel Doubly Linked List: this repo's own LIFO Stack<T>
// holding each level's not-yet-resumed Next pointer, the same "push where DFS should resume,
// descend now" shape FlattenNestedListIteratorTests' pending stack already uses for LC 341's
// nested lists - here descending into Child instead of a NestedInteger's own sublist.
public sealed partial class FlattenAMultilevelDoublyLinkedListTests
{
    [Fact]
    public void Flatten_ChildNestedTwoLevelsDeep_ProducesDepthFirstOrderWithNoChildPointersLeft()
    {
        var n1 = new Node(1);
        var n2 = new Node(2);
        var n3 = new Node(3);
        Link(n1, n2, n3);

        var c1 = new Node(4);
        var c2 = new Node(5);
        Link(c1, c2);
        n2.Child = c1;

        var g1 = new Node(6);
        var g2 = new Node(7);
        Link(g1, g2);
        c1.Child = g1;

        var head = Flatten(n1);

        Assert.Equal([1, 2, 4, 6, 7, 5, 3], ToValues(head));
        Assert.All(ToNodes(head), node => Assert.Null(node.Child));
    }

    [Fact]
    public void Flatten_NoChildren_LeavesFlatListUnchanged()
    {
        var n1 = new Node(1);
        var n2 = new Node(2);
        Link(n1, n2);

        var head = Flatten(n1);

        Assert.Equal([1, 2], ToValues(head));
    }

    [Fact]
    public void Flatten_PreservesPreviousPointersThroughout()
    {
        var n1 = new Node(1);
        var n2 = new Node(2);
        Link(n1, n2);

        var c1 = new Node(3);
        n2.Child = c1;

        var head = Flatten(n1);
        var nodes = ToNodes(head);

        for (var i = 1; i < nodes.Count; i++)
        {
            Assert.Same(nodes[i - 1], nodes[i].Previous);
        }
    }

    private static void Link(params Node[] nodes)
    {
        for (var i = 0; i < nodes.Length - 1; i++)
        {
            nodes[i].Next = nodes[i + 1];
            nodes[i + 1].Previous = nodes[i];
        }
    }

    private static List<int> ToValues(Node? head) => ToNodes(head).Select(node => node.Val).ToList();

    private static List<Node> ToNodes(Node? head)
    {
        var nodes = new List<Node>();

        for (var current = head; current is not null; current = current.Next)
        {
            nodes.Add(current);
        }

        return nodes;
    }

    private static Node? Flatten(Node? head)
    {
        if (head is null)
        {
            return null;
        }

        var pending = new PendingStack();
        var current = head;

        while (current is not null)
        {
            if (current.Child is not null)
            {
                if (current.Next is not null)
                {
                    pending.Push(current.Next);
                }

                current.Next = current.Child;
                current.Child.Previous = current;
                current.Child = null;
            }

            if (current.Next is null && pending.TryPop(out var resumed))
            {
                current.Next = resumed;
                resumed.Previous = current;
            }

            current = current.Next;
        }

        return head;
    }
}
