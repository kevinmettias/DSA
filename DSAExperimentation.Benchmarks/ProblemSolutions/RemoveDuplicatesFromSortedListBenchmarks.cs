using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class RemoveDuplicatesFromSortedListBenchmarks
{
    private int[] _values = null!; [Params(200, 5_000)] public int Length; [GlobalSetup] public void Setup()=>_values=Enumerable.Range(0,Length).Select(i=>i/3).ToArray();
    [Benchmark(Baseline=true)] public int ArrayDistinct()=>_values.Distinct().Count();
    [Benchmark] public int LinkedListCollapse()=>Count(DeleteDuplicates(Build(_values)));
    private static SinglyLinkedListNode<int>? DeleteDuplicates(SinglyLinkedListNode<int>? head){for(var node=head;node is not null;node=node.Next)while(node.Next is not null&&node.Value==node.Next.Value)node.Next=node.Next.Next;return head;}
    private static SinglyLinkedListNode<int>? Build(int[] values){var d=new SinglyLinkedListNode<int>(0);var t=d;foreach(var v in values){t.Next=new SinglyLinkedListNode<int>(v);t=t.Next;}return d.Next;} private static int Count(SinglyLinkedListNode<int>? h){var c=0;for(var n=h;n is not null;n=n.Next)c++;return c;}
}
