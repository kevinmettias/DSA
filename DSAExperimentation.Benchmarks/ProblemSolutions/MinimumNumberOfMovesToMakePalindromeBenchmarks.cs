using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumNumberOfMovesToMakePalindrome;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumNumberOfMovesToMakePalindromeSolution's, the
// same methods MinimumNumberOfMovesToMakePalindromeTests proves correct. The
// measured input is LeetCode's own shape - a string - so neither strategy needs a
// hoisted overload; what [GlobalSetup] owns here is the workload's size and seed.
[MemoryDiagnoser]
public class MinimumNumberOfMovesToMakePalindromeBenchmarks
{
    private const int RandomSeed = 2193;

    // Small alphabet so mismatches - and therefore real swap work - are frequent.
    private const int AlphabetSize = 4;

    [Params(200, 2_000)]
    public int Length;

    private string _value = null!;

    [GlobalSetup]
    public void Setup()
    {
        _value = BuildPalindromeReadyString(Length, RandomSeed);
    }

    // A random string guaranteed rearrangeable into a palindrome (every character
    // count is even, plus at most one odd leftover) - built by mirroring random
    // halves, then fully shuffled so it isn't trivially already a palindrome. The
    // shuffle is a permutation, so every character's total count - and therefore
    // solvability - is preserved.
    private static string BuildPalindromeReadyString(int length, int seed)
    {
        var random = new Random(seed);
        var chars = new char[length];

        FillMirroredHalf(chars, length, random);
        ShuffleInPlace(chars, random);

        return new string(chars);
    }

    private static void FillMirroredHalf(char[] chars, int length, Random random)
    {
        var half = length / 2;

        for (var i = 0; i < half; i++)
        {
            var c = (char)('a' + random.Next(AlphabetSize));
            chars[i] = c;
            chars[length - 1 - i] = c;
        }

        if (length % 2 == 1)
        {
            chars[half] = (char)('a' + random.Next(AlphabetSize));
        }
    }

    private static void ShuffleInPlace(char[] chars, Random random)
    {
        for (var i = chars.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (chars[i], chars[j]) = (chars[j], chars[i]);
        }
    }

    [Benchmark(Baseline = true)]
    public int ListRemoveInsert() =>
        MinimumNumberOfMovesToMakePalindromeSolution.MinMovesByListRemoveInsert(_value);

    [Benchmark]
    public int ArrayIndexedSequenceSwap() =>
        MinimumNumberOfMovesToMakePalindromeSolution.MinMovesByIndexedSequenceSwap(_value);
}
