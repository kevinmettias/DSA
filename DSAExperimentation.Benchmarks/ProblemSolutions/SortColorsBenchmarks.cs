using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class SortColorsBenchmarks
{
    private int[] _values = null!; [Params(200, 5_000)] public int Length; [GlobalSetup] public void Setup()=>_values=Enumerable.Range(0,Length).Select(i=>2-(i%3)).ToArray();
    [Benchmark(Baseline = true)] public int ArraySort(){var copy=_values.ToArray();Array.Sort(copy);return copy[0];}
    [Benchmark] public int ArrayIndexedDutchFlag(){var copy=_values.ToArray();var seq=new ArrayIndexedSequence<int>(copy);var low=0;var mid=0;var high=seq.Length-1;while(mid<=high){if(seq.Get(mid)==0)Swap(seq,low++,mid++);else if(seq.Get(mid)==2)Swap(seq,mid,high--);else mid++;}return copy[0];}
    private static void Swap(ArrayIndexedSequence<int> seq,int first,int second){var temp=seq.Get(first);seq.Set(first,seq.Get(second));seq.Set(second,temp);}
}
