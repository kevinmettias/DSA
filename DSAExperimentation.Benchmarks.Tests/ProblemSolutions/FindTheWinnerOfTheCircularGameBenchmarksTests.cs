using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindTheWinnerOfTheCircularGameBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question, so a harness whose arms disagree is timing two
// different problems. The class has no [Params]-driven Setup at all - both arms build their own
// circle from the two integers LeetCode hands the problem - so there is no workload to rebuild and
// the whole harness is the bare initializer.
public sealed partial class FindTheWinnerOfTheCircularGameBenchmarksTests
{
    private const int SmallestFriendCount = 200;

    [Fact]
    public void ByListRemoval_AgreesWithByQueueRotation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ByQueueRotation(), harness.ByListRemoval());
    }

    [Fact]
    public void ByQueueRotation_AgreesWithByListRemoval()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ByListRemoval(), harness.ByQueueRotation());
    }

    private static FindTheWinnerOfTheCircularGameBenchmarks BuildHarness() =>
        new() { FriendCount = SmallestFriendCount };
}
