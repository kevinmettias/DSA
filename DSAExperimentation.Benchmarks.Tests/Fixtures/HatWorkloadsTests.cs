using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for HatWorkloads (ARCHITECTURE 17.7). The reading depends on every person liking
// a few distinct hats out of one shared small pool, so the (hat, mask) state graph branches and the
// memoized arm has something to win.
public sealed partial class HatWorkloadsTests
{
    private const int PeopleCount = 12;
    private const int HatPoolSize = 10;
    private const int LikedHatsPerPerson = 3;
    private const int Seed = 1434; // LC problem number
    private const int LowestHat = 1;

    [Fact]
    public void BuildLikedHats_PeopleCount_ReturnsOneLikedHatListPerPerson()
    {
        var likedHats = HatWorkloads.BuildLikedHats(PeopleCount, HatPoolSize, LikedHatsPerPerson, Seed);

        Assert.Equal(PeopleCount, likedHats.Length);
        Assert.All(likedHats, liked => Assert.Equal(LikedHatsPerPerson, liked.Length));
    }

    [Fact]
    public void BuildLikedHats_EveryPerson_LikesDistinctHatsFromTheSharedPool()
    {
        var likedHats = HatWorkloads.BuildLikedHats(PeopleCount, HatPoolSize, LikedHatsPerPerson, Seed);

        Assert.All(
            likedHats,
            liked => Assert.Equal(LikedHatsPerPerson, liked.Distinct().Count()));
        Assert.All(
            likedHats,
            liked => Assert.All(liked, hat => Assert.InRange(hat, LowestHat, HatPoolSize)));
    }

    [Fact]
    public void BuildLikedHats_SameSeed_ReturnsTheSameLists() =>
        Assert.Equal(
            AnswerText.Of(HatWorkloads.BuildLikedHats(PeopleCount, HatPoolSize, LikedHatsPerPerson, Seed)),
            AnswerText.Of(HatWorkloads.BuildLikedHats(PeopleCount, HatPoolSize, LikedHatsPerPerson, Seed)));
}
