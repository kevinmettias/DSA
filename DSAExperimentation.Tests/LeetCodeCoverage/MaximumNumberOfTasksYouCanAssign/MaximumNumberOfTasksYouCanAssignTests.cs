using DSAExperimentation.LeetCode.MaximumNumberOfTasksYouCanAssign;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumNumberOfTasksYouCanAssign;

// Harness only. Both strategies - the linear walk down k and the
// BinarySearch.LowerBound bisection over the same greedy feasibility check - live in
// MaximumNumberOfTasksYouCanAssignSolution. This file pins them to LeetCode's
// published examples plus the cases that separate the greedy's two hard choices:
// spending a pill on the weakest qualifying worker rather than the strongest, and
// giving up when no worker reaches the hardest remaining task even with one.
public sealed partial class MaximumNumberOfTasksYouCanAssignTests
{
    public static TheoryData<TaskAssignmentExample> Examples =>
        new()
        {
            // LeetCode's three published examples.
            { new TaskAssignmentExample(Tasks: [3, 2, 1], Workers: [0, 3, 3], Pills: 1, Strength: 1, Expected: 3) },
            { new TaskAssignmentExample(Tasks: [5, 4], Workers: [0, 0, 0], Pills: 1, Strength: 5, Expected: 1) },
            { new TaskAssignmentExample(Tasks: [10, 15, 30], Workers: [0, 10, 10, 10, 10], Pills: 3, Strength: 10, Expected: 2) },

            // The single pill has to go to the weakest worker who can still finish
            // the hardest task, keeping both 6s free for the two easy ones.
            { new TaskAssignmentExample(Tasks: [5, 9, 8, 5, 9], Workers: [1, 6, 4, 2, 6], Pills: 1, Strength: 5, Expected: 3) },

            // No pills and nobody strong enough: not one task can be assigned.
            { new TaskAssignmentExample(Tasks: [5, 5], Workers: [1, 1], Pills: 0, Strength: 3, Expected: 0) },

            // Every worker qualifies unaided, so the answer is capped by the count
            // of tasks rather than by strength.
            { new TaskAssignmentExample(Tasks: [1], Workers: [10], Pills: 0, Strength: 0, Expected: 1) },

            // More tasks than workers caps it the other way round.
            { new TaskAssignmentExample(Tasks: [1, 1, 1], Workers: [1], Pills: 5, Strength: 0, Expected: 1) },

            // The strongest worker must be kept for the harder task even though it
            // could also have taken the easier one.
            { new TaskAssignmentExample(Tasks: [3, 5], Workers: [4, 5], Pills: 0, Strength: 0, Expected: 2) },

            // One pill, and both tasks still get done because the boosted worker
            // covers the hardest one.
            { new TaskAssignmentExample(Tasks: [10, 20], Workers: [5, 15, 15], Pills: 1, Strength: 5, Expected: 2) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxTaskAssignmentByLinearScan_LeetCodeExamples_ReturnsMostAssignableTasks(
        TaskAssignmentExample example)
    {
        var actual = MaximumNumberOfTasksYouCanAssignSolution.MaxTaskAssignmentByLinearScan(
            example.Tasks, example.Workers, example.Pills, example.Strength);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxTaskAssignmentBySequenceLowerBound_LeetCodeExamples_ReturnsMostAssignableTasks(
        TaskAssignmentExample example)
    {
        var actual = MaximumNumberOfTasksYouCanAssignSolution.MaxTaskAssignmentBySequenceLowerBound(
            example.Tasks, example.Workers, example.Pills, example.Strength);

        Assert.Equal(example.Expected, actual);
    }

    // One example as one argument: the five values that describe a single case. They
    // travel together - a row IS one case - and passed separately they made a
    // five-parameter signature that could only be read by counting commas.
    public readonly record struct TaskAssignmentExample(
        int[] Tasks, int[] Workers, int Pills, int Strength, int Expected);
}
