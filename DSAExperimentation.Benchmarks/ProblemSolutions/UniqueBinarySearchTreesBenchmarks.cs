using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class UniqueBinarySearchTreesBenchmarks
{
    [Params(10,16)] public int Nodes;
    [Benchmark(Baseline=true)] public int Tabulation(){var dp=new int[Nodes+1];dp[0]=dp[1]=1;for(var n=2;n<=Nodes;n++)for(var l=0;l<n;l++)dp[n]+=dp[l]*dp[n-l-1];return dp[Nodes];}
    [Benchmark] public int MemoizedCatalan()=>Memoizer.Memoize<int,int>(Nodes,Catalan);
    private static int Catalan(int n,Func<int,int> count){if(n<=1)return 1;var total=0;for(var l=0;l<n;l++)total+=count(l)*count(n-l-1);return total;}
}
