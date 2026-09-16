using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.SortIntegersByThePowerValue;

// LeetCode 1387. Sort Integers by The Power Value: rank every integer in [lo, hi] by
// its Collatz "power" (steps to reach 1), breaking ties by the integer itself, and
// report the kth.
//
// Both strategies build the same (Power, Value) pairs and differ only in how they
// order them. ValueTuple<int, int> already compares lexicographically, so sorting the
// pairs sorts by Power first and Value second - exactly LeetCode's tie-break rule -
// with no custom comparer on either side (the KClosestPointsToOrigin precedent).
internal static class SortIntegersByThePowerValueSolution
{
    // Collatz step: even -> divide by CollatzDivisor; odd -> CollatzMultiplier * value + 1.
    private const int CollatzDivisor = 2;
    private const int CollatzMultiplier = 3;

    // The textbook answer: an O(n^2) insertion sort of the pairs over a plain array,
    // ordered by Comparer<T>.Default - no sorting primitive from this repo at all. It
    // is the arm the composed strategy below has to justify itself against.
    public static int GetKthByInsertionSort(int lo, int hi, int rank)
    {
        var pairs = BuildPowerPairs(lo, hi);

        for (var i = 1; i < pairs.Length; i++)
        {
            var current = pairs[i];
            var j = i - 1;

            while (j >= 0 && Comparer<(int Power, int Value)>.Default.Compare(pairs[j], current) > 0)
            {
                pairs[j + 1] = pairs[j];
                j--;
            }

            pairs[j + 1] = current;
        }

        return pairs[rank - 1].Value;
    }

    // This repo's own MergeSort over ArrayIndexedSequence - O(n log n) and stable -
    // the same composition SortAnArray and HeightChecker use.
    public static int GetKthByMergeSort(int lo, int hi, int rank)
    {
        var pairs = BuildPowerPairs(lo, hi);

        MergeSort.Sort<(int Power, int Value), ArrayIndexedSequence<(int Power, int Value)>>(
            new ArrayIndexedSequence<(int Power, int Value)>(pairs));

        return pairs[rank - 1].Value;
    }

    // Shared by both strategies so the only thing they differ in is the ordering pass.
    private static (int Power, int Value)[] BuildPowerPairs(int lo, int hi)
    {
        var length = hi - lo + 1;
        var pairs = new (int Power, int Value)[length];

        for (var i = 0; i < length; i++)
        {
            var value = lo + i;
            pairs[i] = (PowerOf(value), value);
        }

        return pairs;
    }

    private static int PowerOf(int value)
    {
        var power = 0;

        while (value != 1)
        {
            var dividesEvenly = value % CollatzDivisor == 0;
            value = dividesEvenly ? DivideByCollatzDivisor(value) : TripleAndIncrement(value);
            power++;
        }

        return power;
    }

    private static int DivideByCollatzDivisor(int value) => value / CollatzDivisor;

    private static int TripleAndIncrement(int value) => (CollatzMultiplier * value) + 1;
}
