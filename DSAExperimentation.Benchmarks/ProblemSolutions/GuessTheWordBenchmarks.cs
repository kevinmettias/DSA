using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Guess the Word (LC 843): filtering the candidate pool IN PLACE with a BCL
// List<string> - removing every word that stops matching by index, back to front -
// vs. this repo's own DynamicArray<string> rebuilding a fresh pool each round via
// Add. In-place removal near the front of a List shifts every trailing element, so a
// round that eliminates many candidates costs O(poolSize^2) instead of O(poolSize);
// rebuilding a fresh DynamicArray costs a single O(poolSize) pass with no shifting.
// Both variants pick the same guess every round (candidate 0, in original wordList
// order), so both converge on the identical secret in the identical number of
// rounds - only the filtering cost differs. WordCount intentionally runs past
// LeetCode's own 100-word cap: match-count filtering converges in single-digit
// rounds regardless of pool size, so the O(poolSize^2) penalty per round - not the
// round count - is what needs a large pool to separate from constant overhead and
// show clearly.
[MemoryDiagnoser]
public class GuessTheWordBenchmarks
{
    [Params(100, 1_000)]
    public int WordCount;

    private string[] _wordList = null!;
    private string _secret = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(843);
        var seen = new HashSet<string>();
        var words = new List<string>();

        while (words.Count < WordCount)
        {
            var word = RandomWord(random, 6);

            if (seen.Add(word))
            {
                words.Add(word);
            }
        }

        _wordList = words.ToArray();
        _secret = _wordList[WordCount / 2];
    }

    private static string RandomWord(Random random, int length)
    {
        var chars = new char[length];

        for (var i = 0; i < length; i++)
        {
            chars[i] = (char)('a' + random.Next(26));
        }

        return new string(chars);
    }

    [Benchmark(Baseline = true)]
    public int InPlaceListRemoval()
    {
        var candidates = new List<string>(_wordList);
        var guessCount = 0;

        while (true)
        {
            var guess = candidates[0];
            var matches = MatchCount(guess, _secret);
            guessCount++;

            if (matches == guess.Length)
            {
                return guessCount;
            }

            for (var i = candidates.Count - 1; i >= 0; i--)
            {
                if (MatchCount(candidates[i], guess) != matches)
                {
                    candidates.RemoveAt(i);
                }
            }
        }
    }

    [Benchmark]
    public int ShrinkingCandidatePool()
    {
        var candidates = new DynamicArray<string>();

        foreach (var word in _wordList)
        {
            candidates.Add(word);
        }

        var guessCount = 0;

        while (true)
        {
            var guess = candidates.Get(0);
            var matches = MatchCount(guess, _secret);
            guessCount++;

            if (matches == guess.Length)
            {
                return guessCount;
            }

            var next = new DynamicArray<string>();

            for (var i = 0; i < candidates.Count; i++)
            {
                var candidate = candidates.Get(i);

                if (MatchCount(candidate, guess) == matches)
                {
                    next.Add(candidate);
                }
            }

            candidates = next;
        }
    }

    private static int MatchCount(string first, string second)
    {
        var count = 0;

        for (var i = 0; i < first.Length; i++)
        {
            if (first[i] == second[i])
            {
                count++;
            }
        }

        return count;
    }
}
