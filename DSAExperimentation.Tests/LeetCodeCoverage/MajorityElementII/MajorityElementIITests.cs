using DSAExperimentation.LeetCode.MajorityElementII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MajorityElementII;

// Harness only. Both strategies here are MajorityElementIISolution's - this file
// pins them to LeetCode's published examples plus the no-winner and two-winner
// edges the original two-example test left uncovered. Order is unconstrained by
// the problem statement, so both sides are sorted before comparing.
public sealed partial class MajorityElementIITests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [3, 2, 3], [3] }, // LC's example 1
            { [1], [1] }, // LC's example 2 (single element)
            { [1, 2], [1, 2] }, // LC's example 3 - both values cross n/3 when n = 2
            { [1, 2, 3, 4, 5, 6], [] }, // every value appears once; nothing crosses n/3
            { [-1, -1, -1, 2, 2], [-1, 2] }, // negative values, two winners
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MajorityByHashMap_LeetCodeExamples_ReturnsElementsOverNThirds(int[] nums, int[] expected) =>
        Assert.Equal(expected.Order(), MajorityElementIISolution.MajorityByHashMap(nums).Order());

    [Theory]
    [MemberData(nameof(Examples))]
    public void MajorityByExtendedBoyerMooreVoting_LeetCodeExamples_ReturnsElementsOverNThirds(
        int[] nums, int[] expected) =>
        Assert.Equal(expected.Order(), MajorityElementIISolution.MajorityByExtendedBoyerMooreVoting(nums).Order());

    // The two arms are competing strategies for one question, and the voting arm is the
    // one that leans on the "at most two such values" bound the other ignores - so the
    // property worth pinning is that both name the same set, not merely that each agrees
    // with the expectation beside it.
    [Theory]
    [MemberData(nameof(Examples))]
    public void Majority_AgreeOnEveryExample(int[] nums, int[] expected) =>
        Assert.Equal(
            MajorityElementIISolution.MajorityByHashMap(nums).Order(),
            MajorityElementIISolution.MajorityByExtendedBoyerMooreVoting(nums).Order());
}
