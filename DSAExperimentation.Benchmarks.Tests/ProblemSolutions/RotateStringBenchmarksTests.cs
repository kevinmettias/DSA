using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RotateStringBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a restart-on-mismatch scan of source + source against the KMP
// prefix-function search - so a harness whose arms disagree is timing two different problems. Setup
// builds both strings from Length alone, so the same Length must rebuild the same pair; otherwise
// two published numbers were never comparable in the first place.
//
// The class comment fixes what both arms must answer: the source is a run of 'a' ending in 'b' and
// the goal is a run of 'a' ending in 'c'. A rotation of the source is a cyclic shift of it, so every
// rotation still holds exactly one 'b' and no 'c' at all - the goal is not a rotation of the source,
// which is the decisive answer asserted here rather than read back out of an arm.
public sealed partial class RotateStringBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().CanRotateByNaiveScan(),
            BuildHarness().CanRotateByNaiveScan());

    [Fact]
    public void CanRotateByNaiveScan_GoalOutsideTheRotationSet_AgreesWithThePrefixFunction()
    {
        var harness = BuildHarness();

        Assert.False(harness.CanRotateByNaiveScan());
        Assert.Equal(harness.CanRotateByPrefixFunction(), harness.CanRotateByNaiveScan());
    }

    [Fact]
    public void CanRotateByPrefixFunction_GoalOutsideTheRotationSet_AgreesWithTheNaiveScan()
    {
        var harness = BuildHarness();

        Assert.False(harness.CanRotateByPrefixFunction());
        Assert.Equal(harness.CanRotateByNaiveScan(), harness.CanRotateByPrefixFunction());
    }

    private static RotateStringBenchmarks BuildHarness()
    {
        var harness = new RotateStringBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
