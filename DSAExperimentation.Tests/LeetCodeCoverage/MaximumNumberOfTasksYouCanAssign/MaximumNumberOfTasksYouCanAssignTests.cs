using DSAExperimentation.LeetCode.MaximumNumberOfTasksYouCanAssign;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumNumberOfTasksYouCanAssign;

// Harness only: both strategies - the linear walk down k and the
// BinarySearch.LowerBound bisection over the same greedy feasibility check - live in
// MaximumNumberOfTasksYouCanAssignSolution. This file pins them to LeetCode's
// published examples plus the cases that separate the greedy's two hard choices:
// spending a pill on the weakest qualifying worker rather than the strongest, and
// giving up when no worker reaches the hardest remaining task even with one.
public sealed class MaximumNumberOfTasksYouCanAssignTests
{
    public static TheoryData<int[], int[], int, int, int> Examples =>
        new()
        {
            // LeetCode's three published examples.
            { [3, 2, 1], [0, 3, 3], 1, 1, 3 },
            { [5, 4], [0, 0, 0], 1, 5, 1 },
            { [10, 15, 30], [0, 10, 10, 10, 10], 3, 10, 2 },

            // The single pill has to go to the weakest worker who can still finish
            // the hardest task, keeping both 6s free for the two easy ones.
            { [5, 9, 8, 5, 9], [1, 6, 4, 2, 6], 1, 5, 3 },

            // No pills and nobody strong enough: not one task can be assigned.
            { [5, 5], [1, 1], 0, 3, 0 },

            // Every worker qualifies unaided, so the answer is capped by the count
            // of tasks rather than by strength.
            { [1], [10], 0, 0, 1 },

            // More tasks than workers caps it the other way round.
            { [1, 1, 1], [1], 5, 0, 1 },

            // The strongest worker must be kept for the harder task even though it
            // could also have taken the easier one.
            { [3, 5], [4, 5], 0, 0, 2 },

            // One pill, and both tasks still get done because the boosted worker
            // covers the hardest one.
            { [10, 20], [5, 15, 15], 1, 5, 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxTaskAssignmentByLinearScan_LeetCodeExamples_ReturnsMostAssignableTasks(
        int[] tasks, int[] workers, int pills, int strength, int expected) =>
        Assert.Equal(
            expected,
            MaximumNumberOfTasksYouCanAssignSolution.MaxTaskAssignmentByLinearScan(tasks, workers, pills, strength));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxTaskAssignmentBySequenceLowerBound_LeetCodeExamples_ReturnsMostAssignableTasks(
        int[] tasks, int[] workers, int pills, int strength, int expected) =>
        Assert.Equal(
            expected,
            MaximumNumberOfTasksYouCanAssignSolution.MaxTaskAssignmentBySequenceLowerBound(
                tasks, workers, pills, strength));
}
