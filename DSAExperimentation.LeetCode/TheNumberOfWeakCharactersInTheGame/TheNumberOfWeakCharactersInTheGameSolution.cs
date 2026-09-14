using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.TheNumberOfWeakCharactersInTheGame;

// LeetCode 1996. The Number of Weak Characters in the Game: a character is weak when
// some other character beats it on BOTH attack and defense, strictly. Count them.
//
// The two strategies differ in what they do about "some other character": the baseline
// asks the question literally, comparing every character against every other, while the
// composed strategy orders the roster so that one linear pass can answer it. Sorting by
// attack descending with defense ASCENDING on ties is the load-bearing half of that -
// it guarantees that every character already scanned either outranks the current one on
// attack or ties it with a smaller defense, so the running maximum defense can never be
// contributed by an equal-attack character and "defense below the running maximum" is
// exactly "weak". The sort is this repo's own MergeSort over ArrayIndexedSequence, the
// same composition LC 1857's greedy uses.
internal static class TheNumberOfWeakCharactersInTheGameSolution
{
    // The textbook answer: the problem's definition read straight off the page, every
    // character against every other, O(n^2). Deliberately written without this repo's
    // primitives - it is the arm the sort-and-scan strategy has to justify itself
    // against.
    public static int NumberOfWeakCharactersByPairwiseComparison(int[][] properties) =>
        NumberOfWeakCharactersByPairwiseComparison(CharacterRoster.Build(properties));

    public static int NumberOfWeakCharactersByPairwiseComparison(CharacterRoster roster)
    {
        var weakCount = 0;

        for (var index = 0; index < roster.Count; index++)
        {
            if (IsBeatenOnBoth(roster, index))
            {
                weakCount++;
            }
        }

        return weakCount;
    }

    // No self-comparison guard is needed: nothing strictly beats itself on either axis.
    private static bool IsBeatenOnBoth(CharacterRoster roster, int index)
    {
        var (attack, defense) = roster.At(index);

        for (var other = 0; other < roster.Count; other++)
        {
            var (otherAttack, otherDefense) = roster.At(other);

            if (otherAttack > attack && otherDefense > defense)
            {
                return true;
            }
        }

        return false;
    }

    // Sort once, then one pass tracking the best defense seen among the strictly
    // stronger-attack characters already processed.
    public static int NumberOfWeakCharactersBySortThenScan(int[][] properties) =>
        NumberOfWeakCharactersBySortThenScan(CharacterRoster.Build(properties));

    public static int NumberOfWeakCharactersBySortThenScan(CharacterRoster roster)
    {
        var characters = roster.ToSortableArray();
        SortByAttackDescendingDefenseAscending(characters);

        return CountBelowRunningMaximumDefense(characters);
    }

    private static void SortByAttackDescendingDefenseAscending((int Attack, int Defense)[] characters)
    {
        var byAttackDescendingDefenseAscending = Comparer<(int Attack, int Defense)>.Create(
            (a, b) => a.Attack != b.Attack ? b.Attack.CompareTo(a.Attack) : a.Defense.CompareTo(b.Defense));

        MergeSort.Sort<(int Attack, int Defense), ArrayIndexedSequence<(int Attack, int Defense)>>(
            new ArrayIndexedSequence<(int Attack, int Defense)>(characters), byAttackDescendingDefenseAscending);
    }

    private static int CountBelowRunningMaximumDefense((int Attack, int Defense)[] characters)
    {
        var weakCount = 0;
        var maxDefenseSoFar = 0;

        foreach (var (_, defense) in characters)
        {
            if (defense < maxDefenseSoFar)
            {
                weakCount++;
            }

            maxDefenseSoFar = Math.Max(maxDefenseSoFar, defense);
        }

        return weakCount;
    }
}
