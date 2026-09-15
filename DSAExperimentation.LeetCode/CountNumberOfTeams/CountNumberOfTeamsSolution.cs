using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.CountNumberOfTeams;

// LeetCode 1395. Count Number of Teams: how many index triples i < j < k have
// strictly increasing or strictly decreasing ratings.
//
// For every soldier j taken as the middle of a team the count is
// leftLess[j]*rightGreater[j] + leftGreater[j]*rightLess[j], so the whole problem
// is four rank counts per index.
internal static class CountNumberOfTeamsSolution
{
    // A team needs a middle and a right soldier after the leftmost one.
    private const int TrailingElementsNeededForTeam = 2;

    // The smallest rating count that can hold a team at all.
    private const int MinimumTeamSize = 3;

    // The textbook answer: fix i and j, then scan every k after j. Deliberately
    // written without this repo's primitives - it is the arm the composed solution
    // below has to justify itself against.
    public static int CountTeamsByTripleLoop(int[] rating)
    {
        var teams = 0;

        for (var i = 0; i < rating.Length - TrailingElementsNeededForTeam; i++)
        {
            for (var j = i + 1; j < rating.Length - 1; j++)
            {
                teams += CountTeamsWithMiddle(rating, i, j);
            }
        }

        return teams;
    }

    private static int CountTeamsWithMiddle(int[] rating, int i, int j)
    {
        var teams = 0;

        for (var k = j + 1; k < rating.Length; k++)
        {
            if (FormsRankOrderedTeam(rating[i], rating[j], rating[k]))
            {
                teams++;
            }
        }

        return teams;
    }

    // The three ratings run strictly one way - all rising or all falling - which is what
    // makes i, j, k a team.
    private static bool FormsRankOrderedTeam(int first, int middle, int last) =>
        (first < middle && middle < last) || (first > middle && middle > last);

    // Each of the four counts comes from a coordinate-compressed sweep: rank a
    // rating via BinarySearch.LowerBound/UpperBound over the sorted distinct
    // values, tally it in a FenwickTree<int, SumOperation<int>> (this repo's own
    // Binary Indexed Tree), and read the prefix sums back out. The same
    // rank-then-Fenwick-sweep shape CountOfSmallerNumbersAfterSelfSolution uses,
    // run once left-to-right and once right-to-left so every index learns how many
    // soldiers on each side rank below and above it.
    public static int CountTeamsByFenwickSweeps(int[] rating)
    {
        var n = rating.Length;
        if (n < MinimumTeamSize)
        {
            return 0;
        }

        var sortedDistinct = rating.Distinct().OrderBy(value => value).ToArray();
        var sequence = new ArraySequence<int>(sortedDistinct);

        var (leftLess, leftGreater) = SweepCounts(rating, sequence, sortedDistinct.Length, SweepDirection.Forward);
        var (rightLess, rightGreater) = SweepCounts(rating, sequence, sortedDistinct.Length, SweepDirection.Backward);

        var teams = 0;
        for (var j = 0; j < n; j++)
        {
            teams += (leftLess[j] * rightGreater[j]) + (leftGreater[j] * rightLess[j]);
        }

        return teams;
    }

    // One directional sweep shared by both passes: SweepDirection.Forward builds
    // leftLess/leftGreater (how many earlier soldiers rank below/above j),
    // SweepDirection.Backward walks right-to-left over the same indices to build
    // rightLess/rightGreater instead.
    private static (int[] Less, int[] Greater) SweepCounts(
        int[] rating, ArraySequence<int> sequence, int distinctCount, SweepDirection direction)
    {
        var n = rating.Length;
        var less = new int[n];
        var greater = new int[n];
        var tree = new FenwickTree<int, SumOperation<int>>(distinctCount);
        var seenSoFar = 0;

        for (var step = 0; step < n; step++)
        {
            var j = direction == SweepDirection.Forward ? step : IndexFromEnd(step, n);
            var lower = BinarySearch.LowerBound(sequence, rating[j]);
            var upper = BinarySearch.UpperBound(sequence, rating[j]);

            less[j] = lower == 0 ? 0 : tree.PrefixQuery(lower - 1);
            var lessOrEqual = upper == 0 ? 0 : tree.PrefixQuery(upper - 1);
            greater[j] = seenSoFar - lessOrEqual;

            tree.Add(lower, 1);
            seenSoFar++;
        }

        return (less, greater);
    }

    // The index one step back from the end of the ratings, so a Backward pass covers
    // the same positions as a Forward one, in the opposite order.
    private static int IndexFromEnd(int step, int length) => length - 1 - step;

    // Which way one SweepCounts pass walks the ratings: Forward visits index 0 first
    // and so counts the soldiers ranked before j, Backward visits the last index first
    // and counts the ones ranked after it.
    private enum SweepDirection
    {
        Forward,
        Backward,
    }
}
