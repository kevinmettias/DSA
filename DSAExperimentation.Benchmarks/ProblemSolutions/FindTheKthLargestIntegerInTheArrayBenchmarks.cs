using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindTheKthLargestIntegerInTheArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheKthLargestIntegerInTheArraySolution's, the same
// methods FindTheKthLargestIntegerInTheArrayTests proves correct. The workload is
// random digit strings of mixed length, so the numeric order the solution imposes
// (length first, then ordinal) actually differs from string's own lexicographic one,
// and K stays small so the size-k heap's log factor is on K rather than on Length.
[MemoryDiagnoser]
public class FindTheKthLargestIntegerInTheArrayBenchmarks
{
    private const int K = 10;
    private const int RandomSeed = 1985; // LC problem number
    private const int MaxDigitCountExclusive = 12;
    private const int NonZeroDigitChoices = 9; // leading digit is drawn from 1-9
    private const int DecimalDigitRadix = 10; // remaining digits are drawn from 0-9

    [Params(500, 10_000)]
    public int Length;

    private string[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => RandomDigitString(random)).ToArray();
    }

    private static string RandomDigitString(Random random)
    {
        var digitCount = random.Next(1, MaxDigitCountExclusive);
        var digits = new char[digitCount];
        digits[0] = (char)('1' + random.Next(NonZeroDigitChoices));

        for (var i = 1; i < digitCount; i++)
        {
            digits[i] = (char)('0' + random.Next(DecimalDigitRadix));
        }

        return new string(digits);
    }

    [Benchmark(Baseline = true)]
    public string FullSort() => FindTheKthLargestIntegerInTheArraySolution.KthLargestNumberByFullSort(_values, K);

    [Benchmark]
    public string SizeKMinHeap() =>
        FindTheKthLargestIntegerInTheArraySolution.KthLargestNumberBySizeKMinHeap(_values, K);
}
