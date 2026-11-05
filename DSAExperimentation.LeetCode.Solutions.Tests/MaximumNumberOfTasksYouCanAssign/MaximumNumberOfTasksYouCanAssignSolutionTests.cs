using DSAExperimentation.LeetCode.Solutions.MaximumNumberOfTasksYouCanAssign;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumNumberOfTasksYouCanAssign;

// Harness only. Both strategies - the linear walk down k and the
// MonotonePredicateSearch bisection over the same greedy feasibility check - live in
// MaximumNumberOfTasksYouCanAssignSolution. This file pins them to the cases that
// separate the greedy's two hard choices - spending a pill on the weakest qualifying
// worker rather than the strongest, and giving up when no worker reaches the
// hardest remaining task even with one - and to answers capped by the count of
// tasks or of workers. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class MaximumNumberOfTasksYouCanAssignSolutionTests
{
    public static TheoryData<TaskAssignmentCase> Cases =>
        new()
        {
            // The single pill has to go to the weakest worker who can still finish
            // the hardest task, keeping both 6s free for the two easy ones.
            { new TaskAssignmentCase(Tasks: [5, 9, 8, 5, 9], Workers: [1, 6, 4, 2, 6], Pills: 1, Strength: 5, Expected: 3) },

            // No pills and nobody strong enough: not one task can be assigned.
            { new TaskAssignmentCase(Tasks: [5, 5], Workers: [1, 1], Pills: 0, Strength: 3, Expected: 0) },

            // Every worker qualifies unaided, so the answer is capped by the count
            // of tasks rather than by strength.
            { new TaskAssignmentCase(Tasks: [1], Workers: [10], Pills: 0, Strength: 0, Expected: 1) },

            // More tasks than workers caps it the other way round.
            { new TaskAssignmentCase(Tasks: [1, 1, 1], Workers: [1], Pills: 5, Strength: 0, Expected: 1) },

            // The strongest worker must be kept for the harder task even though it
            // could also have taken the easier one.
            { new TaskAssignmentCase(Tasks: [3, 5], Workers: [4, 5], Pills: 0, Strength: 0, Expected: 2) },

            // One pill, and both tasks still get done because the boosted worker
            // covers the hardest one.
            { new TaskAssignmentCase(Tasks: [10, 20], Workers: [5, 15, 15], Pills: 1, Strength: 5, Expected: 2) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxTaskAssignmentByLinearScan_Cases_ReturnsMostAssignableTasks(
        TaskAssignmentCase testCase)
    {
        var actual = MaximumNumberOfTasksYouCanAssignSolution.MaxTaskAssignmentByLinearScan(
            testCase.Tasks, testCase.Workers, testCase.Pills, testCase.Strength);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxTaskAssignmentByPredicateSearch_Cases_ReturnsMostAssignableTasks(
        TaskAssignmentCase testCase)
    {
        var actual = MaximumNumberOfTasksYouCanAssignSolution.MaxTaskAssignmentByPredicateSearch(
            testCase.Tasks, testCase.Workers, testCase.Pills, testCase.Strength);

        Assert.Equal(testCase.Expected, actual);
    }

    // One case as one argument: the five values that describe it. They
    // travel together - a row IS one case - and passed separately they made a
    // five-parameter signature that could only be read by counting commas.
    public readonly record struct TaskAssignmentCase(
        int[] Tasks, int[] Workers, int Pills, int Strength, int Expected);
}
