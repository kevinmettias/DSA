using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.LinkedListCycleII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LinkedListCycleIISolution's, the same methods
// LinkedListCycleIITests proves correct. [GlobalSetup] builds a Length-node list
// whose tail rejoins the head, forcing both strategies to walk the full cycle
// before they can answer; neither strategy mutates the list, so it is built once
// and shared across iterations.
//
// Returns object, not SinglyLinkedListNode<int> - the node type is internal, so a
// public [Benchmark] method cannot name it as a return type (CS0050).
[MemoryDiagnoser]
public class LinkedListCycleIIBenchmarks
{
    [Params(200, 5_000)] public int Length;

    private SinglyLinkedListNode<int>? _head;

    [GlobalSetup]
    public void Setup() => _head = BuildCyclicList(Length);

    [Benchmark(Baseline = true)]
    public object? VisitedSet() => LinkedListCycleIISolution.DetectCycleByVisitedSet(_head);

    [Benchmark]
    public object? FloydCycleDetection() => LinkedListCycleIISolution.DetectCycleByFloydCycleDetection(_head);

    private static SinglyLinkedListNode<int> BuildCyclicList(int length)
    {
        var head = new SinglyLinkedListNode<int>(0);
        var tail = head;

        for (var i = 1; i < length; i++)
        {
            tail.Next = new SinglyLinkedListNode<int>(i);
            tail = tail.Next;
        }

        tail.Next = head;
        return head;
    }
}
