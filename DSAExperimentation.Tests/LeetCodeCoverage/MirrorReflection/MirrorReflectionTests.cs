using DSAExperimentation.LeetCode.MirrorReflection;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MirrorReflection;

// Harness only. Both strategies are MirrorReflectionSolution's - this file pins
// them to LeetCode's published examples plus pairs that exercise the GCD reduction
// itself (non-coprime (p, q), and q = p), including the O(p) unfolding simulation
// that used to live unasserted in the benchmark.
public sealed class MirrorReflectionTests
{
    public static TheoryData<int, int, int> Examples =>
        new()
        {
            { 2, 1, 2 },
            { 3, 1, 1 },
            { 3, 2, 0 },
            { 1, 1, 1 },
            { 4, 2, 2 },
            { 4, 3, 2 },
            { 5, 2, 0 },
            { 6, 4, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReceptorBySimulatedUnfolding_LeetCodeExamples_ReturnsReceptorTheRayHits(
        int p, int q, int expected) =>
        Assert.Equal(expected, MirrorReflectionSolution.ReceptorBySimulatedUnfolding(p, q));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReceptorByGcdReduction_LeetCodeExamples_ReturnsReceptorTheRayHits(
        int p, int q, int expected) =>
        Assert.Equal(expected, MirrorReflectionSolution.ReceptorByGcdReduction(p, q));
}
