using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.FindTheNumberOfPossibleWaysForAnEvent;

// LeetCode 3317. Find the Number of Possible Ways for an Event: performerCount
// performers each choose one of stageCount stages (some stages may end up empty),
// every non-empty band then gets a score in [1, maxScore], and two events differ if
// any performer's stage or any band's score differs. Count the ways, modulo 1e9+7.
//
// Both strategies group by "how many stages end up non-empty" - call that j - since
// the score choices only depend on j (maxScore^j ways once j bands exist), not on
// which performer went where. They differ in how they get from performerCount
// performers to a count per j: the brute-force arm actually enumerates every
// stageCount^n stage assignment and tallies distinct stages used per assignment; the
// composed arm recognizes the per-j counts as a textbook DP recurrence and hands it
// to this repo's own Memoizer instead of hand-writing the table and its iteration
// order.
internal static class FindTheNumberOfPossibleWaysForAnEventSolution
{
    // Every one of the stageCount^n stage assignments, tallied by how many distinct
    // stages it used and scored with a plain repeated-multiplication power - the
    // definition read literally, and the arm the memoized DP has to beat.
    public static int NumberOfWaysByBruteForceEnumeration(int performerCount, int stageCount, int maxScore)
    {
        var stages = new int[performerCount];
        var total = CountFromPerformer(0, stages, stageCount, maxScore);

        return (int)(total % ModularArithmetic.Modulo);
    }

    // Memoizer.Memoize's TResult is the whole row f(performers, 0..stageCount) at
    // once, so one memoized call over "performer count" builds exactly the
    // O(n * stageCount) table the textbook DP describes -
    // f(i, j) = f(i-1, j) * j (add to one of j existing bands)
    //         + f(i-1, j-1) * (stageCount - j + 1) (open a new band) - without
    // hand-rolling the table's iteration order. ModularArithmetic.Power then folds
    // maxScore^j into the final sum the same way Domain.Modular's own doc comment
    // expects any "modulo 1e9+7" counting problem to.
    public static int NumberOfWaysByStagePartitionMemo(int performerCount, int stageCount, int maxScore)
    {
        var waysByStageCount = Memoizer.Memoize<int, long[]>(performerCount, new StagePartitionRows(stageCount));

        var total = 0L;

        for (var stages = 1; stages <= stageCount; stages++)
        {
            var scoreWays = ModularArithmetic.Power(maxScore, stages);
            total = (total + waysByStageCount[stages] * scoreWays) % ModularArithmetic.Modulo;
        }

        return (int)total;
    }

    /// <summary>
    /// The recurrence, named: one performer count's whole row
    /// f(performers, 0..stageCount) at once, built from the row one performer shorter -
    /// each performer either joins one of the j stages already open or opens one of the
    /// stageCount - j + 1 that are still empty.
    /// </summary>
    private sealed class StagePartitionRows(int stageCount) : IRecurrence<int, long[]>
    {
        /// <inheritdoc/>
        public long[] Replay(int state, IRecurrence<int, long[]> rest)
        {
            var row = new long[stageCount + 1];

            if (state == 0)
            {
                row[0] = 1;
                return row;
            }

            var previousRow = rest.Replay(state - 1, rest);

            for (var stages = 1; stages <= stageCount; stages++)
            {
                var joinExistingStage = previousRow[stages] * stages;
                var openNewStage = previousRow[stages - 1] * (stageCount - stages + 1);
                row[stages] = (joinExistingStage + openNewStage) % ModularArithmetic.Modulo;
            }

            return row;
        }
    }

    private static long CountFromPerformer(int performer, int[] stages, int stageCount, int maxScore)
    {
        if (performer == stages.Length)
        {
            return ScoreWaysFor(stages, stageCount, maxScore);
        }

        var total = 0L;

        for (var stage = 0; stage < stageCount; stage++)
        {
            stages[performer] = stage;
            total = (total + CountFromPerformer(performer + 1, stages, stageCount, maxScore)) % ModularArithmetic.Modulo;
        }

        return total;
    }

    private static long ScoreWaysFor(int[] stages, int stageCount, int maxScore)
    {
        var stageUsed = new bool[stageCount];
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
            ways = ways * maxScore % ModularArithmetic.Modulo;
        }

        return ways;
    }
}
