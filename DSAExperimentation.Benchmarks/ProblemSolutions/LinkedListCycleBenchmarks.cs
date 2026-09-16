using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.LinkedListCycle;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LinkedListCycleSolution's, the same methods
// LinkedListCycleTests proves correct. [GlobalSetup] builds a Length-node list
// whose tail rejoins the head, forcing both strategies to walk the full cycle
// before they can answer; neither strategy mutates the list, so it is built once
// and shared across iterations.
[MemoryDiagnoser]
public class LinkedListCycleBenchmarks
{
    private SinglyLinkedListNode<int>? _head;

    [Params(200, 5_000)] public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _head = BuildCyclicList(Length);

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

    [Benchmark(Baseline = true)]
    public bool HasCycleByVisitedSet() => LinkedListCycleSolution.HasCycleByVisitedSet(_head);

    [Benchmark]
    public bool HasCycleByFloydCycleDetection() => LinkedListCycleSolution.HasCycleByFloydCycleDetection(_head);
}
