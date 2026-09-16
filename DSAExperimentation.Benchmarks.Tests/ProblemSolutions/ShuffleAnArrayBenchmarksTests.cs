using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ShuffleAnArrayBenchmarks (ARCHITECTURE 17.9). Both arms shuffle the same
// stored array and each draws its own Random(1) inside the call, so the two arms cannot be
// compared permutation for permutation: the naive arm draws the index of a random *remaining*
// pool entry and removes it, while the Fisher-Yates arm walks from the end swapping against a
// random earlier-or-equal position, so the same seed is consumed by two differently shaped
// streams and legitimately yields two different permutations of the same values. Asserting
// that the permutations are equal would be asserting a property neither arm has, the same way
// GenerateRandomPointInACircle's two unseeded generators cannot be compared point for point.
//
// What both arms genuinely share - and what is asserted below - is that each returns a
// permutation of Setup's stored array and that the two permutations carry the same values.
// That is weaker than arm agreement and is reported as such: it witnesses that both arms
// shuffled the workload they were given, not that they shuffled it identically. Setup builds
// its array from a closed form with no draw from any stream, so the same Length is the whole
// of what pins it.
public sealed partial class ShuffleAnArrayBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameOriginalArray()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        Assert.Equal(
            AnswerText.Of(Sorted(first.RemoveRandomRemaining())),
            AnswerText.Of(Sorted(second.RemoveRandomRemaining())));
    }

    [Fact]
    public void RemoveRandomRemaining_SmallestLength_AgreesWithFisherYatesDynamicArrayOnThePermutedValues()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(Sorted(harness.FisherYatesDynamicArray())),
            AnswerText.Of(Sorted(harness.RemoveRandomRemaining())));
        Assert.Equal(
            AnswerText.Of(OriginalValues()),
            AnswerText.Of(Sorted(harness.RemoveRandomRemaining())));
    }

    [Fact]
    public void FisherYatesDynamicArray_SmallestLength_AgreesWithRemoveRandomRemainingOnThePermutedValues()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(Sorted(harness.RemoveRandomRemaining())),
            AnswerText.Of(Sorted(harness.FisherYatesDynamicArray())));
        Assert.Equal(
            AnswerText.Of(OriginalValues()),
            AnswerText.Of(Sorted(harness.FisherYatesDynamicArray())));
    }

    private static IEnumerable<int> Sorted(int[] shuffled) => shuffled.OrderBy(value => value);

    private static IEnumerable<int> OriginalValues() => Enumerable.Range(0, SmallestLength);

    private static ShuffleAnArrayBenchmarks BuildHarness()
    {
        var harness = new ShuffleAnArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
