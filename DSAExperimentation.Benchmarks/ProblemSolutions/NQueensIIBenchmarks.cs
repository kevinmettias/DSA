using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class NQueensIIBenchmarks
{
    [Params(8)] public int Size;
    [Benchmark(Baseline = true)] public int CountWithArrays() { var cols = new bool[Size]; var d = new bool[2*Size-1]; var a = new bool[2*Size-1]; var count=0; void Search(int row) { if (row==Size) { count++; return; } for (var col=0; col<Size; col++) if (!cols[col] && !d[row-col+Size-1] && !a[row+col]) { cols[col]=d[row-col+Size-1]=a[row+col]=true; Search(row+1); cols[col]=d[row-col+Size-1]=a[row+col]=false; } } Search(0); return count; }
    [Benchmark] public int SameBacktrackingShape() => CountWithArrays();
}
