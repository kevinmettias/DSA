using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class RotateListBenchmarks
{
    private int[] _values = null!;
    [Params(200, 5_000)] public int Length;
    [GlobalSetup] public void Setup() => _values = Enumerable.Range(1, Length).ToArray();
    [Benchmark(Baseline = true)] public int ArrayRotate() { var k = Length / 3; var copy = _values[^k..].Concat(_values[..^k]).ToArray(); return copy[0]; }
    [Benchmark] public int LinkedListRotate() => Count(RotateRight(BuildList(_values), Length / 3));
    private static SinglyLinkedListNode<int>? RotateRight(SinglyLinkedListNode<int>? head, int k) { if (head is null || head.Next is null || k == 0) return head; var length = 1; var tail = head; while (tail.Next is not null) { tail = tail.Next; length++; } var shift = k % length; if (shift == 0) return head; var newTail = head; for (var i = 0; i < length - shift - 1; i++) newTail = newTail.Next!; var newHead = newTail.Next; newTail.Next = null; tail.Next = head; return newHead; }
    private static SinglyLinkedListNode<int>? BuildList(int[] values) { var dummy = new SinglyLinkedListNode<int>(0); var tail = dummy; foreach (var value in values) { tail.Next = new SinglyLinkedListNode<int>(value); tail = tail.Next; } return dummy.Next; }
    private static int Count(SinglyLinkedListNode<int>? head) { var count = 0; for (var node = head; node is not null; node = node.Next) count++; return count; }
}
