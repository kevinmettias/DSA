using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FaultyKeyboard;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FaultyKeyboardSolution's, the same methods
// FaultyKeyboardTests proves correct. Every other character is 'i' so both arms
// are forced through the worst case: FinalStringByReversal pays for a real
// reversal on roughly half the string's length (degrading toward O(n^2)), while
// FinalStringByDeque never reverses anything at all.
[MemoryDiagnoser]
public class FaultyKeyboardBenchmarks
{
    // Excludes 'i' itself, so the non-'i' characters never accidentally trigger a
    // reversal of their own and dilute the deliberate every-other-char ratio.
    private static readonly char[] NonIAlphabet = "abcdefghjklmnopqrstuvwxyz".ToCharArray();

    private const int Seed = 1;

    [Params(200, 5_000)]
    public int Length;

    private string _input = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        var chars = new char[Length];
        chars[0] = NonIAlphabet[random.Next(NonIAlphabet.Length)];

        for (var i = 1; i < Length; i++)
        {
            chars[i] = i % 2 == 0 ? 'i' : NonIAlphabet[random.Next(NonIAlphabet.Length)];
        }

        _input = new string(chars);
    }

    [Benchmark(Baseline = true)]
    public string Reversal() => FaultyKeyboardSolution.FinalStringByReversal(_input);

    [Benchmark]
    public string Deque() => FaultyKeyboardSolution.FinalStringByDeque(_input);
}
