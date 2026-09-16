using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SumOfTwoIntegersBenchmarks (ARCHITECTURE 17.9): both arms are
// SumOfTwoIntegersSolution's - the plain + the problem forbids against the compliant XOR/AND-shift
// carry loop - so a harness whose arms disagree is timing two different questions. Both answer with a
// bare int, and the pair is the class's own fixed second addend against the smaller of its two
// FirstAddend values, so the sum is a constant rather than merely agreed: two arms that were both
// wrong in the same way would otherwise agree.
//
// The class carries no [GlobalSetup] and no [Params]-driven state, so there is no workload to rebuild
// and nothing for a Setup test to pin.
public sealed partial class SumOfTwoIntegersBenchmarksTests
{
    // Mirrors SumOfTwoIntegersBenchmarks' own private SecondAddend.
    private const int SecondAddend = 123_456_789;
    private const int SmallestFirstAddend = 1_000;
    private const int ExpectedSum = SmallestFirstAddend + SecondAddend;

    [Fact]
    public void BuiltInAdd_SmallestAddendPair_AgreesWithTheOtherArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BitwiseCarryLoop(), harness.BuiltInAdd());
        Assert.Equal(ExpectedSum, harness.BuiltInAdd());
    }

    [Fact]
    public void BitwiseCarryLoop_SmallestAddendPair_AgreesWithTheOtherArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BuiltInAdd(), harness.BitwiseCarryLoop());
        Assert.Equal(ExpectedSum, harness.BitwiseCarryLoop());
    }

    private static SumOfTwoIntegersBenchmarks BuildHarness() =>
        new() { FirstAddend = SmallestFirstAddend };
}
