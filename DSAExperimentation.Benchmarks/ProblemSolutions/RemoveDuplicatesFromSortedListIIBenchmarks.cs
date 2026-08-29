using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class RemoveDuplicatesFromSortedListIIBenchmarks
{
    private int[] _values = null!; [Params(200, 5_000)] public int Length; [GlobalSetup] public void Setup()=>_values=Enumerable.Range(0,Length).Select(i=>i/2).ToArray();
    [Benchmark(Baseline=true)] public int ArrayRunFilter()=>_values.GroupBy(x=>x).Where(g=>g.Count()==1).Count();
    [Benchmark] public int LinkedListRunFilter()=>Count(DeleteDuplicates(Build(_values)));
    private static SinglyLinkedListNode<int>? DeleteDuplicates(SinglyLinkedListNode<int>? head){var dummy=new SinglyLinkedListNode<int>(0){Next=head};var previous=dummy;while(previous.Next is not null){var current=previous.Next;var dup=false;while(current.Next is not null&&current.Value==current.Next.Value){dup=true;current=current.Next;}if(dup)previous.Next=current.Next;else previous=previous.Next;}return dummy.Next;}
    private static SinglyLinkedListNode<int>? Build(int[] values){var d=new SinglyLinkedListNode<int>(0);var t=d;foreach(var v in values){t.Next=new SinglyLinkedListNode<int>(v);t=t.Next;}return d.Next;} private static int Count(SinglyLinkedListNode<int>? h){var c=0;for(var n=h;n is not null;n=n.Next)c++;return c;}
}
