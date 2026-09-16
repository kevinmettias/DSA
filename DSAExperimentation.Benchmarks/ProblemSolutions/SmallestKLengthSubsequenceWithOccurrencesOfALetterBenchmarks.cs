using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SmallestKLengthSubsequenceWithOccurrencesOfALetter;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// SmallestKLengthSubsequenceWithOccurrencesOfALetterSolution's, the same methods
// SmallestKLengthSubsequenceWithOccurrencesOfALetterTests proves correct.
// [GlobalSetup] builds the random lowercase string and picks the subsequence length, so
// the comparison is between the O(n*k) naive rescan - which restarts the window scan for
// every one of the subsequenceLength output characters - and the single O(n)
// monotonic-stack sweep, where each character is pushed once and popped at most once
// across the whole string.
[MemoryDiagnoser]
public class SmallestKLengthSubsequenceWithOccurrencesOfALetterBenchmarks
{
    private const char Letter = 'a';
    private const int Repetition = 2;
    private const int LowercaseAlphabetSize = 26;
    private const int SubsequenceLengthDivisor = 2;
    private const int RandomSeed = 1;

    private string _text = "";

    private int _subsequenceLength;
    [Params(500, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var chars = new char[Length];
        for (var i = 0; i < Length; i++)
        {
            chars[i] = (char)('a' + random.Next(0, LowercaseAlphabetSize));
        }

        // Force at least Repetition occurrences of Letter so both strategies stay feasible.
        chars[0] = Letter;
        chars[Length - 1] = Letter;

        _text = new string(chars);
        _subsequenceLength = Length / SubsequenceLengthDivisor;
    }

    [Benchmark(Baseline = true)]
    public string NaiveWindowRescan() =>
        SmallestKLengthSubsequenceWithOccurrencesOfALetterSolution.SmallestSubsequenceByWindowRescan(
            _text, _subsequenceLength, Letter, Repetition);

    [Benchmark]
    public string MonotonicStackSweep() =>
        SmallestKLengthSubsequenceWithOccurrencesOfALetterSolution.SmallestSubsequenceByMonotonicStack(
            _text, _subsequenceLength, Letter, Repetition);
}
