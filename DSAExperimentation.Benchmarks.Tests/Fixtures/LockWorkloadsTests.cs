using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Domain.Locks;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for LockWorkloads (ARCHITECTURE 17.7). The reading depends on the deadend set being
// that many distinct lock states - a random scattering that leaves the graph connected - and on it
// never blocking the two combinations LC 752's own scenario needs to stay reachable.
public sealed partial class LockWorkloadsTests
{
    private const int DeadendCount = 256;
    private const int Seed = 752; // LC problem number
    private const char LowestWheelDigit = '0';

    // LC 752's fixed origin, the combination LockStart.Combination carries in the LeetCode project -
    // repeated here as the value the problem itself fixes, since this project cannot see that type.
    private const string StartCombination = "0000";

    [Fact]
    public void BuildDeadends_DeadendCount_ReturnsExactlyThatManyCombinations() =>
        Assert.Equal(DeadendCount, LockWorkloads.BuildDeadends(DeadendCount, Seed).Length);

    [Fact]
    public void BuildDeadends_EveryCombination_IsDistinct()
    {
        var deadends = LockWorkloads.BuildDeadends(DeadendCount, Seed);

        Assert.Equal(DeadendCount, deadends.Distinct().Count());
    }

    [Fact]
    public void BuildDeadends_EveryCombination_IsAFourDigitLockState()
    {
        var deadends = LockWorkloads.BuildDeadends(DeadendCount, Seed);
        var highestWheelDigit = (char)(LowestWheelDigit + LockWheels.Modulus - 1);

        Assert.All(deadends, deadend => Assert.Equal(LockWheels.Count, deadend.Length));
        Assert.All(
            deadends,
            deadend => Assert.All(deadend, digit => Assert.InRange(digit, LowestWheelDigit, highestWheelDigit)));
    }

    [Fact]
    public void BuildDeadends_NoCombination_BlocksTheStartOrTheFarthestTarget()
    {
        var deadends = LockWorkloads.BuildDeadends(DeadendCount, Seed);

        Assert.DoesNotContain(StartCombination, deadends);
        Assert.DoesNotContain(LockScenario.FarthestTarget, deadends);
    }

    [Fact]
    public void BuildDeadends_SameSeed_ReturnsTheSameDeadends() =>
        Assert.Equal(
            LockWorkloads.BuildDeadends(DeadendCount, Seed),
            LockWorkloads.BuildDeadends(DeadendCount, Seed));
}
