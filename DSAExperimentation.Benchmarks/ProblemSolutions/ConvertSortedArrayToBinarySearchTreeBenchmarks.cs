using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class ConvertSortedArrayToBinarySearchTreeBenchmarks
{
    private int[] _values=null!; [Params(200,5_000)] public int Length; [GlobalSetup] public void Setup()=>_values=Enumerable.Range(0,Length).ToArray();
    [Benchmark(Baseline=true)] public int MidpointBuild()=>Height(Build(_values,0,_values.Length-1));
    [Benchmark] public int BinaryTreeNodeBuild()=>Height(Build(_values,0,_values.Length-1));
    private static BinaryTreeNode<int>? Build(int[] nums,int low,int high){if(low>high)return null;var mid=low+((high-low)/2);return new BinaryTreeNode<int>(nums[mid]){Left=Build(nums,low,mid-1),Right=Build(nums,mid+1,high)};} private static int Height(BinaryTreeNode<int>? n)=>n is null?0:1+Math.Max(Height(n.Left),Height(n.Right));
}
