using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.BoatsToSavePeople;

// LeetCode 881. Boats to Save People: every boat carries at most two people and at
// most `limit` total weight; report the fewest boats that carry everyone.
//
// The heaviest remaining person always needs a boat of their own, so pairing them
// with the lightest person who still fits is never worse than sending them alone -
// that greedy rule is the whole problem. NumRescueBoatsByRepeatedScan applies it
// literally, rescanning for the current heaviest and lightest on every boat
// (O(n^2)). NumRescueBoatsBySortThenTwoPointer sorts the weights ascending with
// this repo's own MergeSort.Sort<Element,TSequence> over an ArrayIndexedSequence
// (AssignCookies' shape, applied to a single array instead of two) so the same
// greedy rule becomes one O(n) two-pointer pass.
internal static class BoatsToSavePeopleSolution
{
    // Deliberately written without this repo's primitives - the baseline the sorted
    // two-pointer pass below has to justify itself against.
    public static int NumRescueBoatsByRepeatedScan(int[] people, int limit)
    {
        var used = new bool[people.Length];
        var remaining = people.Length;
        var boats = 0;

        while (remaining > 0)
        {
            remaining -= FillOneBoat(people, used, limit);
            boats++;
        }

        return boats;
    }

    // Seats the heaviest unseated person, then the lightest who still fits beside
    // them, and reports how many people that boat took.
    private static int FillOneBoat(int[] weights, bool[] used, int limit)
    {
        var heaviestIndex = FindHeaviestUnused(weights, used);
        used[heaviestIndex] = true;

        var lightestIndex = FindLightestUnusedFitting(weights, used, limit, heaviestIndex);

        if (lightestIndex < 0)
        {
            return 1;
        }

        used[lightestIndex] = true;
        return 2;
    }

    private static int FindHeaviestUnused(int[] weights, bool[] used)
    {
        var heaviestIndex = -1;

        for (var i = 0; i < weights.Length; i++)
        {
            if (!used[i] && (heaviestIndex < 0 || weights[i] > weights[heaviestIndex]))
            {
                heaviestIndex = i;
            }
        }

        return heaviestIndex;
    }

    private static int FindLightestUnusedFitting(
        int[] weights, bool[] used, int limit, int heaviestIndex)
    {
        var lightestIndex = -1;

        for (var i = 0; i < weights.Length; i++)
        {
            if (!used[i] && weights[i] + weights[heaviestIndex] <= limit
                && (lightestIndex < 0 || weights[i] < weights[lightestIndex]))
            {
                lightestIndex = i;
            }
        }

        return lightestIndex;
    }

    public static int NumRescueBoatsBySortThenTwoPointer(int[] people, int limit)
    {
        var sorted = (int[])people.Clone();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var light = 0;
        var heavy = sorted.Length - 1;
        var boats = 0;

        while (light <= heavy)
        {
            if (sorted[light] + sorted[heavy] <= limit)
            {
                light++;
            }

            heavy--;
            boats++;
        }

        return boats;
    }
}
