using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TheNumberOfWeakCharactersInTheGame;

// LeetCode 1996. The Number of Weak Characters in the Game: sort by attack descending,
// defense ascending on ties (so a same-attack neighbor can never falsely trigger the weak
// check - RussianDollEnvelopesTests' width/height tie-break shape, mirrored with the
// tie-break direction that suits a descending primary sort instead) via this repo's own
// MergeSort.Sort<Element,TSequence>, then a single linear scan tracks the running maximum
// defense seen among strictly-higher-attack characters already processed - a character is
// weak exactly when its own defense falls below that running maximum.
public sealed partial class TheNumberOfWeakCharactersInTheGameTests
{
    [Theory]
    [MemberData(nameof(Examples))]
    public void NumberOfWeakCharacters_LeetCodeExamples_MatchesExpectedCount(int[][] properties, int expected)
        => Assert.Equal(expected, NumberOfWeakCharacters(properties));

    public static IEnumerable<object[]> Examples()
    {
        yield return [new[] { new[] { 5, 5 }, new[] { 6, 3 }, new[] { 3, 6 } }, 0];
        yield return [new[] { new[] { 2, 2 }, new[] { 3, 3 } }, 1];
        yield return [new[] { new[] { 1, 5 }, new[] { 10, 4 }, new[] { 4, 3 } }, 1];
    }

    private static int NumberOfWeakCharacters(int[][] properties)
    {
        var items = BuildItems(properties);
        SortByAttackDescendingDefenseAscending(items);

        return CountWeakCharacters(items);
    }

    private static (int Attack, int Defense)[] BuildItems(int[][] properties)
        => properties.Select(property => (Attack: property[0], Defense: property[1])).ToArray();

    private static void SortByAttackDescendingDefenseAscending((int Attack, int Defense)[] items)
    {
        var byAttackDescendingDefenseAscending = Comparer<(int Attack, int Defense)>.Create(
            (a, b) => a.Attack != b.Attack ? b.Attack.CompareTo(a.Attack) : a.Defense.CompareTo(b.Defense));

        MergeSort.Sort<(int Attack, int Defense), ArrayIndexedSequence<(int Attack, int Defense)>>(
            new ArrayIndexedSequence<(int Attack, int Defense)>(items), byAttackDescendingDefenseAscending);
    }

    private static int CountWeakCharacters((int Attack, int Defense)[] items)
    {
        var weakCount = 0;
        var maxDefenseSoFar = 0;

        foreach (var (_, defense) in items)
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
