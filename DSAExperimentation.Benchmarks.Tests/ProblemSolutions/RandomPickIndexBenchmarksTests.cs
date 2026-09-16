using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RandomPickIndexBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a uniformly random index whose value equals Target - so a
// harness whose arms disagree is timing two different problems.
//
// The two arms are NOT comparable value for value, and no assertion here pretends otherwise. Each
// solution class draws from its own unseeded Random, advanced a different number of times per call
// (the reservoir arm once per array element, the grouping arm once per call), so equal running
// totals would mean two different sampling procedures made the same draws - a property neither arm
// has. What both genuinely share is the index set they sample: every Pick(7) must return an index
// whose value is 7, so each arm's mean over PickCalls draws has to sit at the mean of that index
// set. The tolerance is that mean's sampling noise, not numeric slack, and the assertion is weaker
// than arm agreement: it witnesses that both arms sample the matching indices, not that they drew
// the same ones. Reported as such.
public sealed partial class RandomPickIndexBenchmarksTests
{
    private const int SmallestLength = 2_000;
    private const int Target = 7;
    private const int PickCalls = 500;
    private const int ValueUpperBoundExclusive = 50;

    // The seed [GlobalSetup] draws its workload with.
    private const int WorkloadSeed = 1;

    // Sampling noise of the mean of PickCalls uniform draws over the matching index set: its
    // standard error is around 26 indices at this call count, several times inside this band and
    // far narrower than an arm sampling a slice of the array could reach.
    private const double RelativeTolerance = 0.15;

    [Fact]
    public void Setup_SmallestLength_BuildsTheSeededWorkload()
    {
        // _nums is private and both arms draw from an unseeded Random inside the solution, so no
        // pair of arm outputs can witness Setup's determinism; the workload is stated against the
        // expression [GlobalSetup] builds it from instead. Target's presence is what makes every
        // Pick in the two tests below answerable, so it is the property those oracles lean on.
        var workload = ExpectedWorkload();

        Assert.Contains(Target, workload);
        Assert.All(workload, value => Assert.InRange(value, 0, ValueUpperBoundExclusive - 1));
    }

    [Fact]
    public void ReservoirSampling_SmallestLength_ReturnsIndicesOfTheTargetValue() =>
        Assert.InRange(
            MeanIndex(BuildHarness().ReservoirSampling()),
            ExpectedMeanIndex() * (1 - RelativeTolerance),
            ExpectedMeanIndex() * (1 + RelativeTolerance));

    [Fact]
    public void HashMapGrouping_SmallestLength_ReturnsIndicesOfTheTargetValue() =>
        Assert.InRange(
            MeanIndex(BuildHarness().HashMapGrouping()),
            ExpectedMeanIndex() * (1 - RelativeTolerance),
            ExpectedMeanIndex() * (1 + RelativeTolerance));

    private static int[] ExpectedWorkload()
    {
        var random = new Random(WorkloadSeed);

        return [.. Enumerable.Range(0, SmallestLength).Select(_ => random.Next(0, ValueUpperBoundExclusive))];
    }

    private static double ExpectedMeanIndex()
    {
        var matching = MatchingIndices();
        var total = 0L;

        foreach (var index in matching)
        {
            total += index;
        }

        return total / (double)matching.Length;
    }

    private static int[] MatchingIndices()
    {
        var workload = ExpectedWorkload();
        var indices = new List<int>();

        for (var index = 0; index < workload.Length; index++)
        {
            if (workload[index] == Target)
            {
                indices.Add(index);
            }
        }

        return [.. indices];
    }

    private static double MeanIndex(long sumOfIndices) => sumOfIndices / (double)PickCalls;

    private static RandomPickIndexBenchmarks BuildHarness()
    {
        var harness = new RandomPickIndexBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
