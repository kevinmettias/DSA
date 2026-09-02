using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Special Binary String (LC 761): the same recursive split-into-special-pieces
// algorithm for both strategies (SplitIntoPieces is shared, non-differentiating
// scaffolding, the same role FindEmptyCell/IsValidPlacement play across both
// strategies in SudokuSolverBenchmarks), differing only in how each level's
// sibling pieces get sorted descending - the BCL's Array.Sort vs. this repo's own
// MergeSort.Sort over an ArrayIndexedSequence (RussianDollEnvelopesTests'
// composition). PairCount controls how many special substrings (1/0 pairs) the
// generated input packs in, scaling both recursion depth and per-level sort width.
[MemoryDiagnoser]
public class SpecialBinaryStringBenchmarks
{
    private const int RandomSeed = 761;
    private const int MinimalSpecialStringLength = 2;
    private const int CoinFlipOutcomes = 2;
    private const string OneBit = "1";
    private const string ZeroBit = "0";
    private const string MinimalSpecialString = "10";

    [Params(50, 200)]
    public int PairCount;

    private string _input = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _input = GenerateSpecial(PairCount, random);
    }

    [Benchmark(Baseline = true)]
    public string ArraySortBaseline() => MakeLargestSpecialArraySort(_input);

    [Benchmark]
    public string MergeSortPrimitive() => MakeLargestSpecialMergeSort(_input);

    // Builds a valid special binary string containing exactly pairCount 1/0
    // pairs, by recursively either wrapping a smaller special string ("1" + s +
    // "0") or concatenating two independently generated special strings -
    // special strings are closed under both operations, which is also exactly
    // why MakeLargestSpecial's own recursion (splitting into top-level pieces,
    // then recursing on each piece's interior) is well-founded.
    private static string GenerateSpecial(int pairCount, Random random)
    {
        if (pairCount <= 1)
        {
            return MinimalSpecialString;
        }

        if (random.Next(CoinFlipOutcomes) == 0)
        {
            return OneBit + GenerateSpecial(pairCount - 1, random) + ZeroBit;
        }

        var left = random.Next(1, pairCount);
        return GenerateSpecial(left, random) + GenerateSpecial(pairCount - left, random);
    }

    private static string MakeLargestSpecialArraySort(string s)
    {
        if (s.Length <= MinimalSpecialStringLength)
        {
            return s;
        }

        var items = SplitIntoPieces(s, MakeLargestSpecialArraySort).ToArray();
        Array.Sort(items, (a, b) => string.CompareOrdinal(b, a));

        return string.Concat(items);
    }

    private static string MakeLargestSpecialMergeSort(string s)
    {
        if (s.Length <= MinimalSpecialStringLength)
        {
            return s;
        }

        var items = SplitIntoPieces(s, MakeLargestSpecialMergeSort).ToArray();
        var descending = Comparer<string>.Create((a, b) => string.CompareOrdinal(b, a));

        MergeSort.Sort<string, ArrayIndexedSequence<string>>(new ArrayIndexedSequence<string>(items), descending);

        return string.Concat(items);
    }

    private static List<string> SplitIntoPieces(string s, Func<string, string> recurse)
    {
        var pieces = new List<string>();
        var balance = 0;
        var start = 0;

        for (var i = 0; i < s.Length; i++)
        {
            balance += s[i] == '1' ? 1 : -1;

            if (balance == 0)
            {
                var pieceInterior = s.Substring(start + 1, i - start - 1);
                pieces.Add(OneBit + recurse(pieceInterior) + ZeroBit);
                start = i + 1;
            }
        }

        return pieces;
    }
}
