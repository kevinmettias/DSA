using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountSubtreesWithMaxDistanceBetweenCitiesBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for the same question - one BFS per member city per mask against
// the classic two-BFS diameter - so a harness whose arms disagree is timing two different problems,
// not two ways of answering one. Setup derives its tree from one fixed seed, so the same CityCount
// must rebuild the same tree; otherwise two published numbers were never comparable in the first
// place.
//
// The adjacency list is private, but the answer's own length is decided by it: the arms report one
// bucket per distance 1..CityCount - 1, so a tree of the documented size pins that length exactly.
public sealed partial class CountSubtreesWithMaxDistanceBetweenCitiesBenchmarksTests
{
    private const int SmallestCityCount = 10;

    [Fact]
    public void Setup_SameCityCount_RebuildsTheSameTree()
    {
        Assert.Equal(SmallestCityCount - 1, BuildHarness().AllPairsBfsPerMask().Length);
        Assert.Equal(
            AnswerText.Of(BuildHarness().AllPairsBfsPerMask()),
            AnswerText.Of(BuildHarness().AllPairsBfsPerMask()));
    }

    [Fact]
    public void AllPairsBfsPerMask_SeededRecursiveTree_AgreesWithDoubleBfsPerMask()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.DoubleBfsPerMask()), AnswerText.Of(harness.AllPairsBfsPerMask()));
    }

    [Fact]
    public void DoubleBfsPerMask_SeededRecursiveTree_AgreesWithAllPairsBfsPerMask()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.AllPairsBfsPerMask()), AnswerText.Of(harness.DoubleBfsPerMask()));
    }

    // AnswerText.Of rather than OfUnorderedSet: both arms report one bucket per distance, so a
    // value's position is the distance it counts, not an arbitrary outer order.
    private static CountSubtreesWithMaxDistanceBetweenCitiesBenchmarks BuildHarness()
    {
        var harness = new CountSubtreesWithMaxDistanceBetweenCitiesBenchmarks { CityCount = SmallestCityCount };
        harness.Setup();

        return harness;
    }
}
