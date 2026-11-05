using DSAExperimentation.LeetCode.Solutions.MirrorReflection;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MirrorReflection;

// Harness only. Both strategies are MirrorReflectionSolution's - this file pins
// them to pairs that exercise the GCD reduction
// itself (non-coprime (roomSide, rayHeight), and rayHeight = roomSide), including the
// O(roomSide) unfolding simulation that used to live unasserted in the benchmark.
public sealed partial class MirrorReflectionSolutionTests
{
    public static TheoryData<int, int, int> Cases =>
        new()
        {
            { 3, 2, 0 },
            { 1, 1, 1 },
            { 4, 3, 2 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void ReceptorBySimulatedUnfolding_Cases_ReturnsReceptorTheRayHits(
        int roomSide, int rayHeight, int expected)
    {
        var actual = MirrorReflectionSolution.ReceptorBySimulatedUnfolding(roomSide, rayHeight);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void ReceptorByGcdReduction_Cases_ReturnsReceptorTheRayHits(
        int roomSide, int rayHeight, int expected)
    {
        var actual = MirrorReflectionSolution.ReceptorByGcdReduction(roomSide, rayHeight);

        Assert.Equal(expected, actual);
    }
}
