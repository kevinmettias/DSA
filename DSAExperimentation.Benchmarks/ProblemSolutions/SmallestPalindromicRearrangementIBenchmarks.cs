using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SmallestPalindromicRearrangementI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SmallestPalindromicRearrangementISolution's, the
// same methods SmallestPalindromicRearrangementITests proves correct. Builds a
// random half and mirrors it so the generated input is always a genuine
// palindrome, matching LeetCode's own guarantee about text.
[MemoryDiagnoser]
public class SmallestPalindromicRearrangementIBenchmarks
{
    private const int RandomSeed = 3517; private string _text = "";

    // LeetCode problem number

    [Params(200, 100_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var half = new char[Length / 2];

        for (var i = 0; i < half.Length; i++)
        {
            half[i] = (char)('a' + random.Next(26));
        }

        var chars = new char[Length];
        half.CopyTo(chars, 0);

        for (var i = 0; i < half.Length; i++)
        {
            chars[Length - 1 - i] = half[i];
        }

        if (Length % 2 == 1)
        {
            chars[Length / 2] = (char)('a' + random.Next(26));
        }

        _text = new string(chars);
    }

    [Benchmark(Baseline = true)]
    public string CharArrayReverse() => SmallestPalindromicRearrangementISolution.RearrangeByCharArrayReverse(_text);

    [Benchmark]
    public string CharStack() => SmallestPalindromicRearrangementISolution.RearrangeByCharStack(_text);
}
