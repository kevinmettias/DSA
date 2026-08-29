using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class SwapNodesInPairsBenchmarks
{
    [Params(200, 5_000)] public int Length;
    private int[] _values = null!;
    [GlobalSetup] public void Setup() => _values = Enumerable.Range(1, Length).ToArray();
    [Benchmark(Baseline = true)] public int ArrayPairSwap() { var copy = _values.ToArray(); for (var i = 0; i + 1 < copy.Length; i += 2) (copy[i], copy[i + 1]) = (copy[i + 1], copy[i]); return copy[0]; }
    [Benchmark] public int LinkedListPairSwap() => Count(SwapPairs(BuildList(_values)));
    private static SinglyLinkedListNode<int>? SwapPairs(SinglyLinkedListNode<int>? head) { var dummy = new SinglyLinkedListNode<int>(0) { Next = head }; var previous = dummy; while (previous.Next?.Next is not null) { var first = previous.Next; var second = first.Next!; first.Next = second.Next; second.Next = first; previous.Next = second; previous = first; } return dummy.Next; }
    private static SinglyLinkedListNode<int>? BuildList(int[] values) { var dummy = new SinglyLinkedListNode<int>(0); var tail = dummy; foreach (var value in values) { tail.Next = new SinglyLinkedListNode<int>(value); tail = tail.Next; } return dummy.Next; }
    private static int Count(SinglyLinkedListNode<int>? head) { var count = 0; for (var node = head; node is not null; node = node.Next) count++; return count; }
}
