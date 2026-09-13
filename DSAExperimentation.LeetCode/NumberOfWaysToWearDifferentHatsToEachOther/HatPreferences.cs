namespace DSAExperimentation.LeetCode.NumberOfWaysToWearDifferentHatsToEachOther;

// LC 1434's input inverted once: the problem states hats[person] (the hats one
// person likes) but every strategy iterates hats outward, so what both of them
// actually walk is people-who-like-this-hat. Inverting is O(total preferences) and
// would otherwise be charged to whichever method a benchmark measures, so it is
// hoisted into a type of its own - the prepared-input container §17.4 asks for. It
// is deliberately not an IEnumerable, so the solution's LeetCode-shaped
// int[][] overload and its prepared overload can never be ambiguous.
//
// This is a witness for one problem and nothing else - the same reason
// LeetCode/CountWaysToBuildRoomsInAnAntColony keeps its two IFoldAlgebra
// implementations beside its solution instead of in Domain.
internal sealed class HatPreferences
{
    private readonly List<int>[] _peopleByHat;

    private HatPreferences(List<int>[] peopleByHat, int peopleCount, int hatCount)
    {
        _peopleByHat = peopleByHat;
        PeopleCount = peopleCount;
        HatCount = hatCount;
    }

    public int PeopleCount { get; }

    // Hats are numbered 1..HatCount; the DP counts down from HatCount to 0.
    public int HatCount { get; }

    // One bit per person, all set: the state every strategy starts from, meaning
    // "nobody has been given a hat yet".
    public int EveryoneMask => (1 << PeopleCount) - 1;

    public IReadOnlyList<int> PeopleWhoLike(int hat) => _peopleByHat[hat];

    // Hats nobody likes contribute nothing to the recurrence (f(hat, mask) is just
    // f(hat - 1, mask) there), so the highest hat actually named is a sufficient
    // upper bound and keeps the recursion shallower than LeetCode's blanket 1..40
    // constraint would.
    public static HatPreferences Build(int[][] likedHats) =>
        Build(likedHats, HighestLikedHat(likedHats));

    // A benchmark already knows the hat pool it generated from, and stating it keeps
    // the measured recursion depth fixed even when a random workload happens to omit
    // the top hat.
    public static HatPreferences Build(int[][] likedHats, int hatCount)
    {
        var peopleByHat = new List<int>[hatCount + 1];

        for (var hat = 0; hat <= hatCount; hat++)
        {
            peopleByHat[hat] = [];
        }

        for (var person = 0; person < likedHats.Length; person++)
        {
            foreach (var hat in likedHats[person])
            {
                peopleByHat[hat].Add(person);
            }
        }

        return new HatPreferences(peopleByHat, likedHats.Length, hatCount);
    }

    private static int HighestLikedHat(int[][] likedHats)
    {
        var highest = 0;

        foreach (var liked in likedHats)
        {
            foreach (var hat in liked)
            {
                highest = Math.Max(highest, hat);
            }
        }

        return highest;
    }
}
