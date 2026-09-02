using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.FindTheNumberOfPossibleWaysForAnEvent;

// LeetCode 3317. Find the Number of Possible Ways for an Event: n performers each
// choose one of x stages (some stages may end up empty), every non-empty band
// then gets a score in [1, y], and two events differ if any performer's stage or
// any band's score differs. Count the ways, modulo 1e9+7.
//
// Both strategies group by "how many stages end up non-empty" - call that j - since
// the score choices only depend on j (y^j ways once j bands exist), not on which
// performer went where. They differ in how they get from n performers to a count
// per j: the brute-force arm actually enumerates every x^n stage assignment and
// tallies distinct stages used per assignment; the composed arm recognizes the
// per-j counts as a textbook DP recurrence and hands it to this repo's own
// Memoizer instead of hand-writing the table and its iteration order.
internal static class FindTheNumberOfPossibleWaysForAnEventSolution
{
    // Every one of the x^n stage assignments, tallied by how many distinct stages
    // it used and scored with a plain repeated-multiplication power - the
    // definition read literally, and the arm the memoized DP has to beat.
    public static int NumberOfWaysByBruteForceEnumeration(int n, int x, int y)
    {
        var stages = new int[n];
        var total = CountFromPerformer(0, stages, x, y);

        return (int)(total % ModularArithmetic.Modulo);
    }

    private static long CountFromPerformer(int performer, int[] stages, int x, int y)
    {
        if (performer == stages.Length)
        {
            return ScoreWaysFor(stages, x, y);
        }

        var total = 0L;

        for (var stage = 0; stage < x; stage++)
        {
            stages[performer] = stage;
            total = (total + CountFromPerformer(performer + 1, stages, x, y)) % ModularArithmetic.Modulo;
        }

        return total;
    }

    private static long ScoreWaysFor(int[] stages, int x, int y)
    {
        var stageUsed = new bool[x];
        var distinctStages = 0;

        foreach (var stage in stages)
        {
            if (stageUsed[stage])
            {
                continue;
            }

            stageUsed[stage] = true;
            distinctStages++;
        }

        var ways = 1L;

        for (var i = 0; i < distinctStages; i++)
        {
            ways = ways * y % ModularArithmetic.Modulo;
        }

        return ways;
    }

    // Memoizer.Memoize's TResult is the whole row f(performers, 0..x) at once, so
    // one memoized call over "performer count" builds exactly the O(n * x) table
    // the textbook DP describes - f(i, j) = f(i-1, j) * j (add to one of j existing
    // bands) + f(i-1, j-1) * (x - j + 1) (open a new band) - without hand-rolling
    // the table's iteration order. ModularArithmetic.Power then folds y^j into the
    // final sum the same way Domain.Modular's own doc comment expects any "modulo
    // 1e9+7" counting problem to.
    public static int NumberOfWaysByStagePartitionMemo(int n, int x, int y)
    {
        var waysByStageCount = Memoizer.Memoize<int, long[]>(n, (performers, waysWithFewerPerformers) =>
            BuildRow(performers, x, waysWithFewerPerformers));

        var total = 0L;

        for (var stages = 1; stages <= x; stages++)
        {
            var scoreWays = ModularArithmetic.Power(y, stages);
            total = (total + waysByStageCount[stages] * scoreWays) % ModularArithmetic.Modulo;
        }

        return (int)total;
    }

    private static long[] BuildRow(int performers, int x, Func<int, long[]> waysWithFewerPerformers)
    {
        var row = new long[x + 1];

        if (performers == 0)
        {
            row[0] = 1;
            return row;
        }

        var previousRow = waysWithFewerPerformers(performers - 1);

        for (var stages = 1; stages <= x; stages++)
        {
            var joinExistingStage = previousRow[stages] * stages;
            var openNewStage = previousRow[stages - 1] * (x - stages + 1);
            row[stages] = (joinExistingStage + openNewStage) % ModularArithmetic.Modulo;
        }

        return row;
    }
}
