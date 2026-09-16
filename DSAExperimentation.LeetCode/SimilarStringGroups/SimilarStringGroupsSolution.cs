using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.SimilarStringGroups;

// LeetCode 839. Similar String Groups: two words are similar when they are equal, or
// when swapping exactly one pair of positions in the first yields the second. Group
// the words under that relation - transitively, so "tars" and "arts" land together
// through "rats" even though they are not directly similar - and report how many
// groups there are.
//
// Both strategies run the identical O(n^2 * L) pairwise similarity scan: deciding
// WHICH pairs to compare has no faster general strategy here, so the only axis left
// is how "which group is this word already in?" gets answered.
internal static class SimilarStringGroupsSolution
{
    // A similar-but-unequal pair differs in exactly two positions, and those two
    // characters must be each other's swap.
    private const int SwapMismatchCount = 2;

    // The textbook answer: keep the groups themselves as a list of index sets and,
    // for every similar pair, linearly scan that list to find the group each index
    // currently belongs to before merging. Deliberately BCL-only - List<HashSet<int>>
    // and LINQ - because the lookup cost it pays (O(remaining group count) per merge)
    // is exactly what the disjoint-set arm below has to justify itself against.
    public static int CountGroupsByGroupListScan(string[] strs)
    {
        var groups = new List<HashSet<int>>();

        for (var i = 0; i < strs.Length; i++)
        {
            groups.Add([i]);
        }

        for (var i = 0; i < strs.Length; i++)
        {
            for (var j = i + 1; j < strs.Length; j++)
            {
                MergeIfSimilar(groups, strs, i, j);
            }
        }

        return groups.Count;
    }

    private static void MergeIfSimilar(List<HashSet<int>> groups, string[] strs, int firstIndex, int secondIndex)
    {
        if (!IsSimilar(strs[firstIndex], strs[secondIndex]))
        {
            return;
        }

        var groupI = groups.First(g => g.Contains(firstIndex));
        var groupJ = groups.First(g => g.Contains(secondIndex));

        if (groupI != groupJ)
        {
            groupI.UnionWith(groupJ);
            groups.Remove(groupJ);
        }
    }

    // This repo's own DisjointSet over word indices - the same "union whenever two
    // elements are connected, count distinct roots at the end" shape AccountsMerge and
    // RedundantConnection use. Find answers the group-membership question in O(a(n))
    // amortized instead of scanning a shrinking list of groups, and the roots are
    // deduped through Set<int>, which is exactly its stated purpose.
    public static int CountGroupsByDisjointSet(string[] strs)
    {
        var components = new DisjointSet(strs.Length);

        for (var i = 0; i < strs.Length; i++)
        {
            for (var j = i + 1; j < strs.Length; j++)
            {
                if (IsSimilar(strs[i], strs[j]))
                {
                    components.Union(i, j);
                }
            }
        }

        var roots = new Set<int>();

        for (var i = 0; i < strs.Length; i++)
        {
            roots.TryAdd(components.Find(i));
        }

        return roots.Count;
    }

    // The similarity predicate itself, shared by both strategies so the only thing
    // they differ in is the group bookkeeping. It is a plain character walk with no
    // repo primitive in it, so the baseline arm stays textbook.
    private static bool IsSimilar(string first, string second)
    {
        var state = new MismatchState(0, -1, -1);

        for (var i = 0; i < first.Length; i++)
        {
            var next = TrackMismatch(first[i], second[i], i, state);

            if (next is null)
            {
                return false;
            }

            state = next.Value;
        }

        return state.Count == 0
            || (state.Count == SwapMismatchCount
                && first[state.First] == second[state.Second]
                && first[state.Second] == second[state.First]);
    }

    // Null means "already past two mismatches", so the pair can never be similar and
    // the walk can stop early.
    private static MismatchState? TrackMismatch(char firstChar, char secondChar, int index, MismatchState state)
    {
        if (firstChar == secondChar)
        {
            return state;
        }

        var count = state.Count + 1;

        if (count > SwapMismatchCount)
        {
            return null;
        }

        return count == 1
            ? WithFirstMismatch(state, count, index)
            : WithSecondMismatch(state, count, index);
    }

    // The pair's first mismatched position, which nothing needs to swap.
    private static MismatchState WithFirstMismatch(MismatchState state, int count, int index) =>
        state with { Count = count, First = index };

    // The pair's second mismatched position - the one the first must swap with if
    // the two words are to be similar.
    private static MismatchState WithSecondMismatch(MismatchState state, int count, int index) =>
        state with { Count = count, Second = index };

    private readonly record struct MismatchState(int Count, int First, int Second);
}
