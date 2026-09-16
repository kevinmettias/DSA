using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindSumOfArrayProductOfMagicalSequencesBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for one question - walking every magical sequence by
// backtracking against a carry-digit DP that reaches the same sum without enumerating one - so a
// harness whose arms disagree is timing two different problems. Setup draws nums from one fixed
// seed and derives the required set-bit count from SlotCount, so the same SlotCount must rebuild
// the same triple.
public sealed partial class FindSumOfArrayProductOfMagicalSequencesBenchmarksTests
{
    private const int SmallestSlotCount = 4;

    [Fact]
    public void Setup_SameSlotCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BacktrackEnumeration(), BuildHarness().BacktrackEnumeration());

    [Fact]
    public void BacktrackEnumeration_SmallestSlotCount_AgreesWithCarryDigitDp()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CarryDigitDp(), harness.BacktrackEnumeration());
    }

    [Fact]
    public void CarryDigitDp_SmallestSlotCount_AgreesWithBacktrackEnumeration()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BacktrackEnumeration(), harness.CarryDigitDp());
    }

    private static FindSumOfArrayProductOfMagicalSequencesBenchmarks BuildHarness()
    {
        var harness = new FindSumOfArrayProductOfMagicalSequencesBenchmarks { SlotCount = SmallestSlotCount };
        harness.Setup();

        return harness;
    }
}
