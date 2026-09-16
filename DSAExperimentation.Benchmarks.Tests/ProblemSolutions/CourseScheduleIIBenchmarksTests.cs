using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CourseScheduleIIBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - rescanning every remaining course for an available one against
// Kahn's frontier queue - so a harness whose arms disagree is timing two different problems, not two
// ways of answering one. Setup derives the course graph from CourseCount alone, so the same
// CourseCount must rebuild the same DAG; otherwise two published numbers were never comparable in the
// first place.
//
// The course list is private, and the workload's documented shape - a guaranteed-acyclic DAG whose
// every prerequisite edge points from a lower id to a higher one, with fan-out capped at 3 - forces
// the ordering as well: course 0 is the only one with no prerequisite, and after it each course i is
// the only one whose prerequisites 1..i-1 are all placed, so 0, 1, .. CourseCount - 1 is the only
// valid order there is to return. AnswerText.Of rather than OfUnorderedSet: position is the build
// step, which for this workload is part of the answer.
public sealed partial class CourseScheduleIIBenchmarksTests
{
    private const int SmallestCourseCount = 50;

    [Fact]
    public void Setup_SameCourseCount_RebuildsTheSameDag()
    {
        Assert.Equal(EveryCourseInIdOrder(SmallestCourseCount), BuildHarness().NaiveRescan());
        Assert.Equal(
            AnswerText.Of(BuildHarness().NaiveRescan()),
            AnswerText.Of(BuildHarness().NaiveRescan()));
    }

    [Fact]
    public void NaiveRescan_CappedFanOutDag_AgreesWithKahnsTopologicalSort()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.KahnsTopologicalSort()), AnswerText.Of(harness.NaiveRescan()));
    }

    [Fact]
    public void KahnsTopologicalSort_CappedFanOutDag_AgreesWithNaiveRescan()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.NaiveRescan()), AnswerText.Of(harness.KahnsTopologicalSort()));
    }

    private static CourseScheduleIIBenchmarks BuildHarness()
    {
        var harness = new CourseScheduleIIBenchmarks { CourseCount = SmallestCourseCount };
        harness.Setup();

        return harness;
    }

    private static int[] EveryCourseInIdOrder(int courseCount) => [.. Enumerable.Range(0, courseCount)];
}
