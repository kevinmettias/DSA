using System.Numerics;
using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Groups of Strings (LC 2157): the O(n^2) pairwise baseline - for every pair of
// words, decide "connected?" directly from their two 26-bit letter-set masks via
// BitOperations.PopCount (equal, one bit differs, or two bits differ at equal
// popcount) - vs. this repo's HashMap<mask, representativeWordIndex> turning each
// word's O(26^2) delete/replace neighbor candidates into O(1) lookups instead of a
// full O(n) scan against every other word. Both variants union connected words with
// the identical DisjointSet, so the neighbor-discovery strategy is the only axis
// under test - the same isolation SimilarStringGroupsBenchmarks already applies to
// its own "which lookup strategy" comparison. Each word is a random DISTINCT-letter
// subset of the alphabet (this problem's own precondition), so real add/delete/
// replace connections - not just strangers - actually occur.
[MemoryDiagnoser]
public class GroupsOfStringsBenchmarks
{
    private const int RandomSeed = 2157;
    private const int MinWordLength = 3;
    private const int MaxWordLengthExclusive = 9;
    private const int AlphabetSize = 26;
    private const int ReplaceLetterBitDifference = 2;

    [Params(200, 2_000)]
    public int WordCount;

    private int[] _masks = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _masks = new int[WordCount];

        for (var i = 0; i < WordCount; i++)
        {
            _masks[i] = BuildRandomMask(random);
        }
    }

    private static int BuildRandomMask(Random random)
    {
        var alphabet = new char[AlphabetSize];

        for (var i = 0; i < AlphabetSize; i++)
        {
            alphabet[i] = (char)('a' + i);
        }

        Shuffle(alphabet, random);
        var length = random.Next(MinWordLength, MaxWordLengthExclusive);

        var mask = 0;

        for (var i = 0; i < length; i++)
        {
            mask |= 1 << (alphabet[i] - 'a');
        }

        return mask;
    }

    private static void Shuffle(char[] chars, Random random)
    {
        for (var i = chars.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (chars[i], chars[j]) = (chars[j], chars[i]);
        }
    }

    [Benchmark(Baseline = true)]
    public int PairwisePopCountScan()
    {
        var components = new DisjointSet(_masks.Length);

        for (var i = 0; i < _masks.Length; i++)
        {
            for (var j = i + 1; j < _masks.Length; j++)
            {
                if (AreConnected(_masks[i], _masks[j]))
                {
                    components.Union(i, j);
                }
            }
        }

        return CountGroups(components);
    }

    private static bool AreConnected(int first, int second)
    {
        var diff = first ^ second;
        var diffCount = BitOperations.PopCount((uint)diff);

        return diffCount switch
        {
            0 => true,
            1 => true,
            ReplaceLetterBitDifference => BitOperations.PopCount((uint)first) == BitOperations.PopCount((uint)second),
            _ => false,
        };
    }

    [Benchmark]
    public int HashMapNeighborLookup()
    {
        var n = _masks.Length;
        var representativeByMask = new HashMap<int, int>();

        for (var i = 0; i < n; i++)
        {
            if (!representativeByMask.HasKey(_masks[i]))
            {
                representativeByMask.Set(_masks[i], i);
            }
        }

        var components = new DisjointSet(n);

        for (var i = 0; i < n; i++)
        {
            UnionNeighbors(i, _masks[i], representativeByMask, components);
        }

        return CountGroups(components);
    }

    private readonly record struct UnionContext(HashMap<int, int> RepresentativeByMask, DisjointSet Components);

    private static void UnionNeighbors(int i, int mask, HashMap<int, int> representativeByMask, DisjointSet components)
    {
        if (representativeByMask.TryGetValue(mask, out var sameMask) && sameMask != i)
        {
            components.Union(i, sameMask);
        }

        var context = new UnionContext(representativeByMask, components);

        for (var removedBit = 0; removedBit < AlphabetSize; removedBit++)
        {
            UnionNeighborsWithoutBit(i, mask, removedBit, context);
        }
    }

    private static void UnionNeighborsWithoutBit(int i, int mask, int removedBit, UnionContext context)
    {
        if ((mask & (1 << removedBit)) == 0)
        {
            return;
        }

        var withoutLetter = mask & ~(1 << removedBit);
        UnionIfPresent(i, withoutLetter, context.RepresentativeByMask, context.Components);

        for (var addedBit = 0; addedBit < AlphabetSize; addedBit++)
        {
            if ((mask & (1 << addedBit)) != 0)
            {
                continue;
            }

            UnionIfPresent(i, withoutLetter | (1 << addedBit), context.RepresentativeByMask, context.Components);
        }
    }

    private static void UnionIfPresent(int i, int candidateMask, HashMap<int, int> representativeByMask, DisjointSet components)
    {
        if (representativeByMask.TryGetValue(candidateMask, out var other))
        {
            components.Union(i, other);
        }
    }

    private static int CountGroups(DisjointSet components)
    {
        var roots = new HashMap<int, bool>();

        for (var i = 0; i < components.Count; i++)
        {
            roots.Set(components.Find(i), true);
        }

        return roots.Count;
    }
}
