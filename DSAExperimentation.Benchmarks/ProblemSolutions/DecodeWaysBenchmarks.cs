using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class DecodeWaysBenchmarks
{
    private string _value=null!; [Params(20,80)] public int Length; [GlobalSetup] public void Setup()=>_value=new string('1',Length);
    [Benchmark(Baseline=true)] public int Tabulation(){var dp=new int[_value.Length+1];dp[_value.Length]=1;for(var i=_value.Length-1;i>=0;i--){if(_value[i]=='0')continue;dp[i]=dp[i+1];if(i+1<_value.Length&&int.Parse(_value.AsSpan(i,2))<=26)dp[i]+=dp[i+2];}return dp[0];}
    [Benchmark] public int Memoized(){return Memoizer.Memoize<int,int>(0,Decode);int Decode(int i,Func<int,int> dec){if(i==_value.Length)return 1;if(_value[i]=='0')return 0;var total=dec(i+1);if(i+1<_value.Length&&int.Parse(_value.AsSpan(i,2))<=26)total+=dec(i+2);return total;}}
}
