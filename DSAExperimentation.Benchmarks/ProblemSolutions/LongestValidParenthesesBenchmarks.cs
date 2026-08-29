using BenchmarkDotNet.Attributes;
using IndexStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class LongestValidParenthesesBenchmarks
{
    private string _value = null!;
    [Params(200, 5_000)] public int Length;
    [GlobalSetup] public void Setup() => _value = string.Concat(Enumerable.Repeat("(()())", (Length / 6) + 1))[..Length];
    [Benchmark(Baseline = true)] public int DynamicProgrammingArray() { var dp = new int[_value.Length]; var best = 0; for (var i = 1; i < _value.Length; i++) if (_value[i] == ')') { var open = i - dp[i - 1] - 1; if (open >= 0 && _value[open] == '(') { dp[i] = dp[i - 1] + 2 + (open > 0 ? dp[open - 1] : 0); best = Math.Max(best, dp[i]); } } return best; }
    [Benchmark] public int StackScan() { var stack = new IndexStack(); stack.Push(-1); var best = 0; for (var i = 0; i < _value.Length; i++) { if (_value[i] == '(') stack.Push(i); else { stack.TryPop(out _); if (stack.TryPeek(out var start)) best = Math.Max(best, i - start); else stack.Push(i); } } return best; }
}
