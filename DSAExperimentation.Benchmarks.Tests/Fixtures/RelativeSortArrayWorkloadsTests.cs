using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for RelativeSortArrayWorkloads (ARCHITECTURE 17.7). The reading depends on the
// guarantees LC 1122 states - every value inside [0, 1000], arr2's values distinct, every one of them
// present in arr1 - and on arr1 also holding values arr2 does not rank, so both of the strategies'
// branches have work to do.
public sealed partial class RelativeSortArrayWorkloadsTests
{
    private const int Length = 1_000;
    private const int ReferenceLength = 100;
    private const int Seed = 1122; // LC problem number
    private const int MaxArrayValue = 1_000;

    [Fact]
    public void Build_Lengths_MatchTheRequestedArrays()
    {
        var (arr1, arr2) = RelativeSortArrayWorkloads.Build(Length, ReferenceLength, Seed);

        Assert.Equal(Length, arr1.Length);
        Assert.Equal(ReferenceLength, arr2.Length);
    }

    [Fact]
    public void Build_EveryValue_StaysInsideLeetCodesRange()
    {
        var (arr1, arr2) = RelativeSortArrayWorkloads.Build(Length, ReferenceLength, Seed);

        Assert.All(arr1, value => Assert.InRange(value, 0, MaxArrayValue));
        Assert.All(arr2, value => Assert.InRange(value, 0, MaxArrayValue));
    }

    [Fact]
    public void Build_Arr2_HoldsDistinctValuesEveryOneOfWhichIsInArr1()
    {
        var (arr1, arr2) = RelativeSortArrayWorkloads.Build(Length, ReferenceLength, Seed);

        Assert.Distinct(arr2);
        Assert.All(arr2, value => Assert.Contains(value, arr1));
    }

    [Fact]
    public void Build_Arr1_HoldsValuesArr2DoesNotRank()
    {
        var (arr1, arr2) = RelativeSortArrayWorkloads.Build(Length, ReferenceLength, Seed);

        Assert.Contains(arr1, value => !arr2.Contains(value));
    }

    [Fact]
    public void Build_SameSeed_ReturnsTheSameArrays() =>
        Assert.Equal(
            AnswerGraphText.Of(RelativeSortArrayWorkloads.Build(Length, ReferenceLength, Seed).Arr1),
            AnswerGraphText.Of(RelativeSortArrayWorkloads.Build(Length, ReferenceLength, Seed).Arr1));
}
