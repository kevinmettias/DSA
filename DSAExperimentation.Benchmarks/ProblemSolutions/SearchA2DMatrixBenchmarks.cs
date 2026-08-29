using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class SearchA2DMatrixBenchmarks
{
    private int[][] _matrix = null!; [Params(40, 100)] public int Size; [GlobalSetup] public void Setup(){var value=0;_matrix=Enumerable.Range(0,Size).Select(_=>Enumerable.Range(0,Size).Select(_=>value++).ToArray()).ToArray();}
    [Benchmark(Baseline = true)] public bool LinearScan()=>_matrix.Any(row=>Array.IndexOf(row, Size*Size-1)>=0);
    [Benchmark] public bool BinarySearchMatrix()=>BinarySearch.Find<int,MatrixSequence>(new MatrixSequence(_matrix), Size*Size-1) is not null;
    private readonly struct MatrixSequence(int[][] matrix):IRandomAccessSequence<int>{public int Length=>matrix.Length*matrix[0].Length;public int Get(int index)=>matrix[index/matrix[0].Length][index%matrix[0].Length];}
}
