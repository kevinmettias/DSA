using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class SubsetsIIBenchmarks
{
    private int[] _values=null!; [Params(10,14)] public int Length; [GlobalSetup] public void Setup()=>_values=Enumerable.Range(0,Length).Select(i=>i/2).ToArray();
    [Benchmark(Baseline=true)] public int IterativeDedup(){var set=new HashSet<string>{""};foreach(var v in _values.ToHashSet()){} return Backtracking();}
    [Benchmark] public int Backtracking(){Array.Sort(_values);var count=0;var state=new State();Backtrack.Search<State,int>(state,_=>true,s=>Enumerable.Range(s.Start,_values.Length-s.Start).Where(i=>i==s.Start||_values[i]!=_values[i-1]),(s,i)=>{s.Starts.Push(s.Start);s.Start=i+1;},(s,_)=>s.Start=s.Starts.Pop(),_=>count++);return count;}
    private sealed class State{public int Start{get;set;} public Stack<int> Starts{get;}=new();}
}
