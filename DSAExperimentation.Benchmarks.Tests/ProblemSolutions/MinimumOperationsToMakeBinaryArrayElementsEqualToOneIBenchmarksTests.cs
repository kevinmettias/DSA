using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumOperationsToMakeBinaryArrayElementsEqualToOneIBenchmarks (ARCHITECTURE
// 17.9): its two arms are competing strategies for the same question - the fewest 3-wide flips that
// turn the binary array all-ones - so a harness whose arms disagree is timing two different problems.
// Both arms read the same array [GlobalSetup] built, and the array-mutation arm deliberately clones
// before flipping, so one harness can be asked twice in either order and still see the same input:
// the comparison covers both arms and, through the second call, the non-destructive promise the clone
// makes. Setup draws the array from one fixed seed and pins its last three elements to 1, so the same
// Length must rebuild the same array.
public sealed partial class MinimumOperationsToMakeBinaryArrayElementsEqualToOneIBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().FlipParityWindow(), BuildHarness().FlipParityWindow());

    [Fact]
    public void ArrayMutation_SameBinaryRun_AgreesWithFlipParityWindow()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FlipParityWindow(), harness.ArrayMutation());
    }

    [Fact]
    public void FlipParityWindow_SameBinaryRun_AgreesWithArrayMutation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ArrayMutation(), harness.FlipParityWindow());
    }

    private static MinimumOperationsToMakeBinaryArrayElementsEqualToOneIBenchmarks BuildHarness()
    {
        var harness = new MinimumOperationsToMakeBinaryArrayElementsEqualToOneIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
