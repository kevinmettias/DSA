using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.TwoCityScheduling;

// LeetCode 1029. Two City Scheduling: send exactly half of the 2n people to city A
// and half to city B at minimum total cost.
//
// Flying everyone to B costs sum(bCost); upgrading person i to A instead changes the
// bill by (aCost - bCost). So the cheapest split is simply the n most negative
// changes: sort by (aCost - bCost) ascending, send the first half to A. Both
// strategies run that identical O(n log n) greedy and differ only in the sort
// primitive - the same "same algorithm, different sort primitive" pairing
// QueueReconstructionByHeightSolution uses. TwoCityCostMinimumByArraySortGreedy is
// the textbook arm on BCL Array.Sort; TwoCityCostMinimumByMergeSortGreedy composes
// this repo's own MergeSort.Sort<Element,TSequence> over an ArrayIndexedSequence.
//
// Each strategy has a second overload taking the people already projected out of
// LeetCode's int[][] shape, so a benchmark can charge that projection to
// [GlobalSetup]; (int ACost, int BCost)[] cannot bind to the int[][] overload, so
// the pair is never ambiguous.
internal static class TwoCitySchedulingSolution
{
    private const int CitySplitDivisor = 2; // exactly half go to city A per LC 1029

    public static int TwoCityCostMinimumByArraySortGreedy(int[][] costs) =>
        TwoCityCostMinimumByArraySortGreedy(BuildPeople(costs));

    // The textbook arm: BCL Array.Sort with an inline comparison. Deliberately
    // written without this repo's primitives - it is the arm the MergeSort strategy
    // below has to justify itself against.
    public static int TwoCityCostMinimumByArraySortGreedy((int ACost, int BCost)[] people)
    {
        var items = ((int ACost, int BCost)[])people.Clone();
        Array.Sort(items, (a, b) => (a.ACost - a.BCost).CompareTo(b.ACost - b.BCost));

        return SumCheapestHalfFirst(items);
    }

    public static int TwoCityCostMinimumByMergeSortGreedy(int[][] costs) =>
        TwoCityCostMinimumByMergeSortGreedy(BuildPeople(costs));

    // This repo's own MergeSort.Sort<Element,TSequence> over an ArrayIndexedSequence,
    // the same custom-comparer shape QueueReconstructionByHeightSolution exercises.
    public static int TwoCityCostMinimumByMergeSortGreedy((int ACost, int BCost)[] people)
    {
        var items = ((int ACost, int BCost)[])people.Clone();
        var byCostDifferenceAscending = Comparer<(int ACost, int BCost)>.Create(
            (a, b) => (a.ACost - a.BCost).CompareTo(b.ACost - b.BCost));

        MergeSort.Sort<(int ACost, int BCost), ArrayIndexedSequence<(int ACost, int BCost)>>(
            new ArrayIndexedSequence<(int ACost, int BCost)>(items), byCostDifferenceAscending);

        return SumCheapestHalfFirst(items);
    }

    private static (int ACost, int BCost)[] BuildPeople(int[][] costs) =>
        costs.Select(c => (ACost: c[0], BCost: c[1])).ToArray();

    // Already ordered by (aCost - bCost) ascending: the first half flies to A, the
    // rest to B.
    private static int SumCheapestHalfFirst((int ACost, int BCost)[] people)
    {
        var toCityA = people.Length / CitySplitDivisor;
        var total = 0;

        for (var i = 0; i < people.Length; i++)
        {
            total += i < toCityA ? people[i].ACost : people[i].BCost;
        }

        return total;
    }
}
