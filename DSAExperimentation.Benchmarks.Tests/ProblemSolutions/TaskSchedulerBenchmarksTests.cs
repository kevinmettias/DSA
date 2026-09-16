using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TaskSchedulerBenchmarks (ARCHITECTURE 17.9): both arms are
// TaskSchedulerSolution's - the tick-by-tick rescan of the flat frequency array against this repo's
// HashMap/Heap/Queue composition - so a harness whose arms disagree is timing two different questions.
// Both answer with a bare int over a seeded task sequence and the class's own fixed cooldown, so a
// rebuild at the same TaskCount has to produce the same interval.
public sealed partial class TaskSchedulerBenchmarksTests
{
    private const int SmallestTaskCount = 2_000;

    [Fact]
    public void Setup_SameTaskCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().ArrayScan(), BuildHarness().ArrayScan());

    [Fact]
    public void ArrayScan_SeededTaskSequence_AgreesWithTheOtherArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CooldownHeap(), harness.ArrayScan());
    }

    [Fact]
    public void CooldownHeap_SeededTaskSequence_AgreesWithTheOtherArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ArrayScan(), harness.CooldownHeap());
    }

    private static TaskSchedulerBenchmarks BuildHarness()
    {
        var harness = new TaskSchedulerBenchmarks { TaskCount = SmallestTaskCount };
        harness.Setup();

        return harness;
    }
}
