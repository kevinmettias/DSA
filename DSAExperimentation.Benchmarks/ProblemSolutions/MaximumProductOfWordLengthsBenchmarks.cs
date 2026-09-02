using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumProductOfWordLengths;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumProductOfWordLengthsSolution's, the same
// methods MaximumProductOfWordLengthsTests proves correct. Words are split into
// two disjoint-alphabet halves ('a'-'m' vs. 'n'-'z') so every cross-half pair is
// guaranteed to share no letter, forcing CharacterScan's inner double loop
// through its full unmatched worst case instead of exiting early on the first
// shared letter.
[MemoryDiagnoser]
public class MaximumProductOfWordLengthsBenchmarks
{
    // LC problem number, reused as the fixed benchmark-data seed.
    private const int RandomSeed = 318;

    // Splits generated words between the 'a'-'m' and 'n'-'z' alphabet halves.
    private const int AlphabetHalfDivisor = 2;

    private const int MinWordLength = 4;
    private const int MaxWordLengthExclusive = 11;

    // 'a'-'m' and 'n'-'z' are each 13 letters wide (half of the 26-letter alphabet).
    private const int AlphabetHalfSize = 13;

    [Params(100, 800)]
    public int WordCount;

    private string[] _words = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _words = Enumerable.Range(0, WordCount).Select(i => NextRandomWord(random, i)).ToArray();
    }

    private static string NextRandomWord(Random random, int index)
    {
        var alphabetStart = index % AlphabetHalfDivisor == 0 ? 'a' : 'n';
        var length = random.Next(MinWordLength, MaxWordLengthExclusive);
        return RandomWord(random, alphabetStart, length);
    }

    private static string RandomWord(Random random, char alphabetStart, int length)
        => new(Enumerable.Range(0, length).Select(_ => (char)(alphabetStart + random.Next(AlphabetHalfSize))).ToArray());

    [Benchmark(Baseline = true)]
    public int CharacterScan() => MaximumProductOfWordLengthsSolution.MaxProductByCharacterScan(_words);

    [Benchmark]
    public int BitmaskHashMap() => MaximumProductOfWordLengthsSolution.MaxProductByBitmaskHashMap(_words);
}
