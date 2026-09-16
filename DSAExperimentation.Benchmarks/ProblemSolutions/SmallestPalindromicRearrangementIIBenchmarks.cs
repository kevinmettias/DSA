using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SmallestPalindromicRearrangementII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SmallestPalindromicRearrangementIISolution's, the same
// methods SmallestPalindromicRearrangementIITests proves correct. Rank stays modest
// regardless of Length: the backtracking baseline's cost tracks how many complete
// arrangements it must walk before the Rank-th one, not the alphabet's full
// arrangement count, so this is the size that keeps it a fair, terminating
// comparison rather than a demonstration that unbounded rank is intractable for it.
// A small 4-letter alphabet keeps the half's own arrangement count comfortably above
// Rank at both sizes, so both arms always find a real answer.
[MemoryDiagnoser]
public class SmallestPalindromicRearrangementIIBenchmarks
{
    private const int RandomSeed = 3518; // LeetCode problem number
    private const int Rank = 500;
    private const int HalfAlphabetSize = 4;

    private string _palindrome = "";

    [Params(20, 200)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var half = new char[Length / 2];

        for (var i = 0; i < half.Length; i++)
        {
            half[i] = (char)('a' + random.Next(HalfAlphabetSize));
        }

        var chars = new char[Length];
        half.CopyTo(chars, 0);

        for (var i = 0; i < half.Length; i++)
        {
            chars[Length - 1 - i] = half[i];
        }

        _palindrome = new string(chars);
    }

    [Benchmark(Baseline = true)]
    public string BacktrackingRank() =>
        SmallestPalindromicRearrangementIISolution.RearrangeByBacktrackingRank(_palindrome, Rank);

    [Benchmark]
    public string CountingGreedy() =>
        SmallestPalindromicRearrangementIISolution.RearrangeByCountingGreedy(_palindrome, Rank);
}
