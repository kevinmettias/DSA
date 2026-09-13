using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.GuessTheWord;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are GuessTheWordSolution's, the same methods
// GuessTheWordTests proves correct - filtering the candidate pool IN PLACE with a BCL
// List<string> vs. this repo's own DynamicArray<string> rebuilding a fresh pool each
// round. In-place removal near the front of a List shifts every trailing element, so
// a round that eliminates many candidates costs O(poolSize^2) instead of O(poolSize).
// Both arms pick the same guess every round, so both converge on the identical secret
// in the identical number of rounds - only the filtering cost differs.
//
// WordCount intentionally runs past LeetCode's own 100-word cap: match-count
// filtering converges in single-digit rounds regardless of pool size, so the
// per-round penalty - not the round count - is what needs a large pool to separate
// from constant overhead. The generated pool is LeetCode's own string[] shape, so the
// only thing hoisted into [GlobalSetup] is generating it; a fresh SecretWordMaster is
// constructed per invocation because the guess counter is per-run state.
[MemoryDiagnoser]
public class GuessTheWordBenchmarks
{
    private const int RandomSeed = 843; // LC problem number
    private const int WordLength = 6;
    private const int MiddleIndexDivisor = 2;
    private const int AlphabetSize = 26;

    [Params(100, 1_000)]
    public int WordCount;

    private string[] _wordList = null!;
    private string _secret = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var seen = new HashSet<string>();
        var words = new List<string>();

        while (words.Count < WordCount)
        {
            var word = RandomWord(random, WordLength);

            if (seen.Add(word))
            {
                words.Add(word);
            }
        }

        _wordList = words.ToArray();
        _secret = _wordList[WordCount / MiddleIndexDivisor];
    }

    private static string RandomWord(Random random, int length)
    {
        var chars = new char[length];

        for (var i = 0; i < length; i++)
        {
            chars[i] = (char)('a' + random.Next(AlphabetSize));
        }

        return new string(chars);
    }

    [Benchmark(Baseline = true)]
    public string InPlaceListRemoval() =>
        GuessTheWordSolution.FindSecretWordByInPlaceListRemoval(_wordList, new SecretWordMaster(_secret));

    [Benchmark]
    public string ShrinkingCandidatePool() =>
        GuessTheWordSolution.FindSecretWordByShrinkingPool(_wordList, new SecretWordMaster(_secret));
}
