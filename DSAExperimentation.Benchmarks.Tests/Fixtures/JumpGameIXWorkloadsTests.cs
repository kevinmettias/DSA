using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for JumpGameIXWorkloads (ARCHITECTURE 17.7). The reading depends on LC 3660's value
// array varying enough that the number of components and the split points between them both vary,
// rather than collapsing into one trivially sorted run.
public sealed partial class JumpGameIXWorkloadsTests
{
    private const int Count = 256;
    private const int Seed = 3660; // LC problem number
    private const int MinValue = 1;
    private const int MaxValue = 1_000_000_000;
    private const int SecondPosition = 1;

    [Fact]
    public void Build_Count_ReturnsOneValuePerPosition() =>
        Assert.Equal(Count, JumpGameIXWorkloads.Build(Count, Seed).Length);

    [Fact]
    public void Build_EveryValue_StaysWithinTheClosedRange() =>
        Assert.All(
            JumpGameIXWorkloads.Build(Count, Seed),
            value => Assert.InRange(value, MinValue, MaxValue));

    [Fact]
    public void Build_Values_LeaveADescentSoTheArraySplitsIntoComponents()
    {
        var nums = JumpGameIXWorkloads.Build(Count, Seed);

        Assert.Contains(
            Enumerable.Range(SecondPosition, Count - SecondPosition),
            position => nums[position] < nums[position - 1]);
    }

    [Fact]
    public void Build_SameSeed_ReturnsTheSameValues() =>
        Assert.Equal(
            JumpGameIXWorkloads.Build(Count, Seed),
            JumpGameIXWorkloads.Build(Count, Seed));
}
