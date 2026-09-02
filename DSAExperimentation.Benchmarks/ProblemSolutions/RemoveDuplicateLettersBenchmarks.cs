using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RemoveDuplicateLetters;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RemoveDuplicateLettersSolution's, the same
// methods RemoveDuplicateLettersTests proves correct.
[MemoryDiagnoser]
public class RemoveDuplicateLettersBenchmarks
{
    private const int RandomSeed = 316; // LC problem number
    private const int AlphabetSize = 26;

    [Params(200, 5_000)]
    public int Length;

    private string _letters = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _letters = new string(Enumerable.Range(0, Length).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray());
    }

    [Benchmark(Baseline = true)]
    public string RecursiveSplit() => RemoveDuplicateLettersSolution.SmallestSubsequenceByRecursiveSplit(_letters);

    [Benchmark]
    public string StackAndSet() => RemoveDuplicateLettersSolution.SmallestSubsequenceByStackAndSet(_letters);
}
