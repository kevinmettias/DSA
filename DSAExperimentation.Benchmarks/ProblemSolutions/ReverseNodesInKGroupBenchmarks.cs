using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class ReverseNodesInKGroupBenchmarks
{
    private const int GroupSize = 4;
    [Params(200, 5_000)] public int Length;
    private int[] _values = null!;
    [GlobalSetup] public void Setup() => _values = Enumerable.Range(1, Length).ToArray();
    [Benchmark(Baseline = true)] public int ArrayGroupReverse() { var copy = _values.ToArray(); for (var i = 0; i + GroupSize <= copy.Length; i += GroupSize) Array.Reverse(copy, i, GroupSize); return copy[0]; }
    [Benchmark] public int LinkedListGroupReverse() => Count(ReverseKGroup(BuildList(_values), GroupSize));
    private static SinglyLinkedListNode<int>? ReverseKGroup(SinglyLinkedListNode<int>? head, int k) { var dummy = new SinglyLinkedListNode<int>(0) { Next = head }; var groupPrevious = dummy; while (TryGetKth(groupPrevious, k, out var kth)) { var groupNext = kth.Next; SinglyLinkedListNode<int>? previous = groupNext; var current = groupPrevious.Next; while (current != groupNext) { var next = current!.Next; current.Next = previous; previous = current; current = next; } var oldGroupHead = groupPrevious.Next!; groupPrevious.Next = kth; groupPrevious = oldGroupHead; } return dummy.Next; }
    private static bool TryGetKth(SinglyLinkedListNode<int> groupPrevious, int k, out SinglyLinkedListNode<int> kth) { SinglyLinkedListNode<int>? node = groupPrevious; for (var i = 0; i < k && node is not null; i++) node = node.Next; kth = node!; return node is not null; }
    private static SinglyLinkedListNode<int>? BuildList(int[] values) { var dummy = new SinglyLinkedListNode<int>(0); var tail = dummy; foreach (var value in values) { tail.Next = new SinglyLinkedListNode<int>(value); tail = tail.Next; } return dummy.Next; }
    private static int Count(SinglyLinkedListNode<int>? head) { var count = 0; for (var node = head; node is not null; node = node.Next) count++; return count; }
}
