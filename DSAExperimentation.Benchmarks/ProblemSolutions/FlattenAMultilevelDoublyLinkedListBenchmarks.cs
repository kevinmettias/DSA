using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Flatten a Multilevel Doubly Linked List (LC 430): a brute-force approach that rescans from
// head after every single splice (the classic "no bookkeeping" naive flatten, O(n^2) once every
// node has a child) vs. this repo's own LIFO Stack<T> holding each level's not-yet-resumed Next
// pointer, letting one forward pass splice every child list in O(n) - the same "push where DFS
// should resume, descend now" shape FlattenNestedListIteratorBenchmarks already uses for LC 341.
// Each [Benchmark] rebuilds a fresh copy since flattening is destructive (Child pointers are
// cleared in place), matching RotateImageBenchmarks' per-invocation Clone convention.
[MemoryDiagnoser]
public class FlattenAMultilevelDoublyLinkedListBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    [Benchmark(Baseline = true)]
    public int BruteForceRescanFromHead() => CountNodes(BruteForceFlatten(BuildList(Length)));

    [Benchmark]
    public int StackBasedOnePass() => CountNodes(StackFlatten(BuildList(Length)));

    private static Node BuildList(int length)
    {
        var head = new Node(0);
        var current = head;

        for (var i = 1; i < length; i++)
        {
            var next = new Node(i);
            current.Next = next;
            next.Previous = current;
            current = next;
        }

        // Every node but the last gets a single-node child, forcing one splice per node.
        for (var node = head; node.Next is not null; node = node.Next)
        {
            node.Child = new Node(-1);
        }

        return head;
    }

    private static int CountNodes(Node? head)
    {
        var count = 0;
        for (var node = head; node is not null; node = node.Next)
        {
            count++;
        }

        return count;
    }

    private static Node? BruteForceFlatten(Node? head)
    {
        while (true)
        {
            var splicePoint = FindNodeWithChild(head);
            if (splicePoint is null)
            {
                break;
            }

            var after = splicePoint.Next;
            var childHead = splicePoint.Child!;
            splicePoint.Next = childHead;
            childHead.Previous = splicePoint;
            splicePoint.Child = null;

            var tail = childHead;
            while (tail.Next is not null)
            {
                tail = tail.Next;
            }

            tail.Next = after;
            if (after is not null)
            {
                after.Previous = tail;
            }
        }

        return head;
    }

    private static Node? FindNodeWithChild(Node? head)
    {
        for (var node = head; node is not null; node = node.Next)
        {
            if (node.Child is not null)
            {
                return node;
            }
        }

        return null;
    }

    private static Node? StackFlatten(Node? head)
    {
        if (head is null)
        {
            return null;
        }

        var pending = new DSAExperimentation.DataStructures.Stack.Stack<Node>();
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

    private sealed class Node(int val)
    {
        public int Val { get; } = val;

        public Node? Previous { get; set; }

        public Node? Next { get; set; }

        public Node? Child { get; set; }
    }
}
