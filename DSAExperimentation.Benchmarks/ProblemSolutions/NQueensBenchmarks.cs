using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class NQueensBenchmarks
{
    [Params(8)] public int Size;
    [Benchmark(Baseline = true)] public int SpecializedCount() => CountSpecialized(Size);
    [Benchmark] public int BacktrackingCount() => CountBacktrack(Size);
    private static int CountSpecialized(int n) { var cols = new bool[n]; var d = new bool[2*n-1]; var a = new bool[2*n-1]; var count=0; void Search(int row) { if (row==n) { count++; return; } for (var col=0; col<n; col++) if (!cols[col] && !d[row-col+n-1] && !a[row+col]) { cols[col]=d[row-col+n-1]=a[row+col]=true; Search(row+1); cols[col]=d[row-col+n-1]=a[row+col]=false; } } Search(0); return count; }
    private static int CountBacktrack(int n) { var count=0; var s=new State(n); Backtrack.Search<State,int>(s, x=>x.Row==n, x=>x.Row==n?[]:Enumerable.Range(0,n).Where(x.CanPlace), (x,c)=>x.Place(c), (x,c)=>x.Remove(c), _=>count++); return count; }
    private sealed class State(int n) { private readonly bool[] _cols=new bool[n]; private readonly bool[] _diag=new bool[2*n-1]; private readonly bool[] _anti=new bool[2*n-1]; public int Row{get;private set;} public bool CanPlace(int c)=>!_cols[c]&&!_diag[Row-c+n-1]&&!_anti[Row+c]; public void Place(int c){_cols[c]=_diag[Row-c+n-1]=_anti[Row+c]=true;Row++;} public void Remove(int c){Row--;_cols[c]=_diag[Row-c+n-1]=_anti[Row+c]=false;} }
}
