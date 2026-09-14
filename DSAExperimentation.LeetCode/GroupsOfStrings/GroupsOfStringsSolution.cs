using System.Numerics;
using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.GroupsOfStrings;

// LeetCode 2157. Groups of Strings: every word has distinct letters (the problem's
// own constraint), so a word IS a 26-bit letter set. Two words are connected when
// their sets are equal, differ by exactly one letter (add or delete), or differ by
// exactly one letter in each direction (replace). Connectivity is transitive, and
// the answer is [number of groups, size of the largest group].
//
// The two strategies differ only in how "which other words is this one connected
// to?" is answered - every pair compared, or every candidate neighbour of a mask
// enumerated directly and looked up.
internal static class GroupsOfStringsSolution
{
    private const int AlphabetSize = 26;

    // Two set bits differing at equal popcount is a REPLACE; one bit differing is
    // an add or a delete; zero is equality.
    private const int ReplaceLetterBitDifference = 2;

    // The textbook answer: compare every pair of letter sets and decide
    // connectedness straight from the two masks. Deliberately BCL-only - a plain
    // parent array with its own find/union walk and an int[] of root sizes, no
    // repo primitive anywhere inside - since it is the arm the neighbour-lookup
    // strategy below has to justify itself against. Only the prepared mask
    // sequence it is handed is a repo type.
    public static int[] GroupSizesByPairwisePopCount(string[] words) =>
        GroupSizesByPairwisePopCount(LetterSetMasks(words));

    public static int[] GroupSizesByPairwisePopCount(ArraySequence<int> masks)
    {
        var parent = new int[masks.Length];

        for (var i = 0; i < parent.Length; i++)
        {
            parent[i] = i;
        }

        for (var i = 0; i < masks.Length; i++)
        {
            for (var j = i + 1; j < masks.Length; j++)
            {
                if (AreConnected(masks.Get(i), masks.Get(j)))
                {
                    Union(parent, i, j);
                }
            }
        }

        return TallyRootSizes(parent);
    }

    private static bool AreConnected(int first, int second)
    {
        var differingLetters = BitOperations.PopCount((uint)(first ^ second));

        return differingLetters switch
        {
            0 => true,
            1 => true,
            ReplaceLetterBitDifference =>
                BitOperations.PopCount((uint)first) == BitOperations.PopCount((uint)second),
            _ => false,
        };
    }

    private static int Find(int[] parent, int id)
    {
        while (parent[id] != id)
        {
            id = parent[id];
        }

        return id;
    }

    private static void Union(int[] parent, int first, int second)
    {
        var firstRoot = Find(parent, first);
        var secondRoot = Find(parent, second);

        if (firstRoot != secondRoot)
        {
            parent[firstRoot] = secondRoot;
        }
    }

    // Roots are word indices, so the baseline can tally into a plain array rather
    // than a map - the textbook shape, and the reason this arm needs no repo
    // container at all.
    private static int[] TallyRootSizes(int[] parent)
    {
        var sizes = new int[parent.Length];
        var groups = 0;
        var largest = 0;

        for (var i = 0; i < parent.Length; i++)
        {
            var root = Find(parent, i);
            sizes[root]++;

            if (sizes[root] == 1)
            {
                groups++;
            }

            if (sizes[root] > largest)
            {
                largest = sizes[root];
            }
        }

        return [groups, largest];
    }

    // This repo's own HashMap<mask, firstWordWithIt> plus DisjointSet: a mask's
    // neighbours are enumerable directly - clear one set bit (delete), then
    // clear-and-set a different bit (replace) - so each word's O(26^2) candidates
    // become O(1) lookups instead of an O(n) scan against every other word. That
    // "first-seen owner" role is the same one AccountsMerge's
    // HashMap<email, accountIndex> plays, and the union-then-tally-by-root shape is
    // SimilarStringGroups' own.
    public static int[] GroupSizesByHashMapNeighbors(string[] words) =>
        GroupSizesByHashMapNeighbors(LetterSetMasks(words));

    public static int[] GroupSizesByHashMapNeighbors(ArraySequence<int> masks)
    {
        var representativeByMask = BuildRepresentativeByMask(masks);
        var components = new DisjointSet(masks.Length);

        for (var i = 0; i < masks.Length; i++)
        {
            UnionNeighbors(i, masks.Get(i), representativeByMask, components);
        }

        return TallyComponentSizes(masks.Length, components);
    }

    private static HashMap<int, int> BuildRepresentativeByMask(ArraySequence<int> masks)
    {
        var representativeByMask = new HashMap<int, int>();

        for (var i = 0; i < masks.Length; i++)
        {
            if (!representativeByMask.HasKey(masks.Get(i)))
            {
                representativeByMask.Set(masks.Get(i), i);
            }
        }

        return representativeByMask;
    }

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
        UnionIfPresent(i, withoutLetter, context);

        for (var addedBit = 0; addedBit < AlphabetSize; addedBit++)
        {
            if ((mask & (1 << addedBit)) == 0)
            {
                UnionIfPresent(i, withoutLetter | (1 << addedBit), context);
            }
        }
    }

    private static void UnionIfPresent(int i, int candidateMask, UnionContext context)
    {
        if (context.RepresentativeByMask.TryGetValue(candidateMask, out var other))
        {
            context.Components.Union(i, other);
        }
    }

    private static int[] TallyComponentSizes(int count, DisjointSet components)
    {
        var sizeByRoot = new HashMap<int, int>();
        var groups = 0;
        var largest = 0;

        for (var i = 0; i < count; i++)
        {
            var root = components.Find(i);
            var size = sizeByRoot.TryGetValue(root, out var existing) ? existing + 1 : 1;
            sizeByRoot.Set(root, size);

            if (size == 1)
            {
                groups++;
            }

            if (size > largest)
            {
                largest = size;
            }
        }

        return [groups, largest];
    }

    // The prepared input both hoisted overloads take: one 26-bit letter set per
    // word, so a benchmark's [GlobalSetup] can charge the mask pass to setup rather
    // than to the grouping being measured. ArraySequence keeps that overload
    // unambiguous against the string[] one while still indexing straight into the
    // underlying array.
    public static ArraySequence<int> LetterSetMasks(string[] words)
    {
        var masks = new int[words.Length];

        for (var i = 0; i < words.Length; i++)
        {
            masks[i] = LetterSetMask(words[i]);
        }

        return new ArraySequence<int>(masks);
    }

    private static int LetterSetMask(string word)
    {
        var mask = 0;

        foreach (var letter in word)
        {
            mask |= 1 << (letter - 'a');
        }

        return mask;
    }

    // Groups the two things every neighbour-candidate lookup needs beyond the
    // candidate mask itself, so the delete/replace walk stays inside this repo's
    // parameter-count limit.
    private readonly record struct UnionContext(HashMap<int, int> RepresentativeByMask, DisjointSet Components);
}
