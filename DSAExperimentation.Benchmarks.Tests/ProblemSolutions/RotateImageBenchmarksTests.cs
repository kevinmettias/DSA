using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RotateImageBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - transposing then reversing each row against pushing each ring
// onto a stack and writing it back - so a harness whose arms disagree is timing two different
// problems. Setup draws the matrix from one fixed seed and size, so the same Size must rebuild the
// same matrix; otherwise two published numbers were never comparable in the first place.
//
// The solution rotates in place, so each arm clones the pristine matrix inside the measured call and
// the hoisted matrix is never written to - one harness is therefore safe to call twice in either
// order and the single-harness rule holds. The answer is a rotation of a randomly filled square
// matrix, so the two arms are reconciled against each other rather than against a re-derivation of
// the rotated cells.
public sealed partial class RotateImageBenchmarksTests
{
    private const int SmallestSize = 50;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameMatrix() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().ArrayReverse()),
            AnswerText.Of(BuildHarness().ArrayReverse()));

    [Fact]
    public void ArrayReverse_SeededMatrix_AgreesWithStackReverse()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.StackReverse()), AnswerText.Of(harness.ArrayReverse()));
    }

    [Fact]
    public void StackReverse_SeededMatrix_AgreesWithArrayReverse()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.ArrayReverse()), AnswerText.Of(harness.StackReverse()));
    }

    private static RotateImageBenchmarks BuildHarness()
    {
        var harness = new RotateImageBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
