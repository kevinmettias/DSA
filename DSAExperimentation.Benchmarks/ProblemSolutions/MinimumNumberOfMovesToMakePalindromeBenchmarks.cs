using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Number of Moves to Make Palindrome (LC 2193): the identical greedy
// two-pointer algorithm run over two different backing representations for the
// same "indexed access, move one element" contract - a BCL List<char> moved via
// RemoveAt/Insert vs. this repo's ArrayIndexedSequence<char> moved via direct
// Get/Set swaps. Both are the same O(n^2) algorithm computing the same answer;
// this is the concrete "performance independence" case ARCHITECTURE.md SS8 names -
// same syntactic contract, different real cost purely from the representation.
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
    public int ListRemoveInsert()
    {
        var chars = new List<char>(_value);
        var moves = 0;
        int i = 0, j = chars.Count - 1;

        while (i < j)
        {
            if (chars[i] == chars[j])
            {
                i++;
                j--;
                continue;
            }

            (i, j, moves) = ResolveMismatch(chars, i, j, moves);
        }

        return moves;
    }

    private static (int I, int J, int Moves) ResolveMismatch(List<char> chars, int i, int j, int moves)
    {
        var k = j;

        while (k > i && chars[k] != chars[i])
        {
            k--;
        }

        if (k == i)
        {
            var mid = chars[i];
            chars.RemoveAt(i);
            chars.Insert(i + 1, mid);
            return (i, j, moves + 1);
        }

        var match = chars[k];
        chars.RemoveAt(k);
        chars.Insert(j, match);
        return (i + 1, j - 1, moves + (j - k));
    }

    [Benchmark]
    public int ArrayIndexedSequenceSwap()
    {
        var sequence = new ArrayIndexedSequence<char>(_value.ToCharArray());
        var moves = 0;
        int i = 0, j = sequence.Length - 1;

        while (i < j)
        {
            if (sequence.Get(i) == sequence.Get(j))
            {
                i++;
                j--;
                continue;
            }

            (i, j, moves) = ResolveMismatch(sequence, i, j, moves);
        }

        return moves;
    }

    private static (int I, int J, int Moves) ResolveMismatch(ArrayIndexedSequence<char> sequence, int i, int j, int moves)
    {
        var k = j;

        while (k > i && sequence.Get(k) != sequence.Get(i))
        {
            k--;
        }

        if (k == i)
        {
            Swap(sequence, i, i + 1);
            return (i, j, moves + 1);
        }

        while (k < j)
        {
            Swap(sequence, k, k + 1);
            moves++;
            k++;
        }

        return (i + 1, j - 1, moves);
    }

    private static void Swap(ArrayIndexedSequence<char> sequence, int first, int second)
    {
        (var a, var b) = (sequence.Get(first), sequence.Get(second));
        sequence.Set(first, b);
        sequence.Set(second, a);
    }
}
