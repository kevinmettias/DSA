using BenchmarkDotNet.Attributes;
using DigitStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class PlusOneBenchmarks
{
    private int[] _digits = null!; [Params(200, 5_000)] public int Length; [GlobalSetup] public void Setup() => _digits = Enumerable.Repeat(9, Length).ToArray();
    [Benchmark(Baseline = true)] public int ArrayFromEnd() { var result = _digits.ToArray(); for (var i = result.Length - 1; i >= 0; i--) { if (result[i] < 9) { result[i]++; return result.Length; } result[i] = 0; } return result.Length + 1; }
    [Benchmark] public int StackDigits() { var stack = new DigitStack(); var carry = 1; for (var i = _digits.Length - 1; i >= 0; i--) { var sum = _digits[i] + carry; stack.Push(sum % 10); carry = sum / 10; } if (carry > 0) stack.Push(carry); var count=0; while (stack.TryPop(out _)) count++; return count; }
}
