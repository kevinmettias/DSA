using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class PartitionListBenchmarks
{
    private int[] _values=null!; [Params(200,5_000)] public int Length; [GlobalSetup] public void Setup()=>_values=Enumerable.Range(0,Length).Reverse().ToArray();
    [Benchmark(Baseline=true)] public int ArrayPartition()=>_values.Where(x=>x<Length/2).Concat(_values.Where(x=>x>=Length/2)).Count();
    [Benchmark] public int LinkedListPartition()=>Count(Partition(Build(_values),Length/2));
    private static SinglyLinkedListNode<int>? Partition(SinglyLinkedListNode<int>? head,int x){var before=new SinglyLinkedListNode<int>(0);var bt=before;var after=new SinglyLinkedListNode<int>(0);var at=after;for(var n=head;n is not null;){var next=n.Next;n.Next=null;if(n.Value<x){bt.Next=n;bt=n;}else{at.Next=n;at=n;}n=next;}bt.Next=after.Next;return before.Next;}
    private static SinglyLinkedListNode<int>? Build(int[] values){var d=new SinglyLinkedListNode<int>(0);var t=d;foreach(var v in values){t.Next=new SinglyLinkedListNode<int>(v);t=t.Next;}return d.Next;} private static int Count(SinglyLinkedListNode<int>? h){var c=0;for(var n=h;n is not null;n=n.Next)c++;return c;}
}
