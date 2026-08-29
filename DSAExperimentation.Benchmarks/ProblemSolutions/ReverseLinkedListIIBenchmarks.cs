using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class ReverseLinkedListIIBenchmarks
{
    private int[] _values=null!; [Params(200,5_000)] public int Length; [GlobalSetup] public void Setup()=>_values=Enumerable.Range(1,Length).ToArray();
    [Benchmark(Baseline=true)] public int ArrayReverseRange(){var copy=_values.ToArray();Array.Reverse(copy,Length/4,Length/2);return copy[0];}
    [Benchmark] public int LinkedListReverseRange()=>Count(ReverseBetween(Build(_values),Length/4,Length*3/4));
    private static SinglyLinkedListNode<int>? ReverseBetween(SinglyLinkedListNode<int>? head,int left,int right){var d=new SinglyLinkedListNode<int>(0){Next=head};var before=d;for(var i=1;i<left;i++)before=before.Next!;var cur=before.Next;for(var i=0;i<right-left;i++){var moved=cur!.Next!;cur.Next=moved.Next;moved.Next=before.Next;before.Next=moved;}return d.Next;}
    private static SinglyLinkedListNode<int>? Build(int[] values){var d=new SinglyLinkedListNode<int>(0);var t=d;foreach(var v in values){t.Next=new SinglyLinkedListNode<int>(v);t=t.Next;}return d.Next;} private static int Count(SinglyLinkedListNode<int>? h){var c=0;for(var n=h;n is not null;n=n.Next)c++;return c;}
}
