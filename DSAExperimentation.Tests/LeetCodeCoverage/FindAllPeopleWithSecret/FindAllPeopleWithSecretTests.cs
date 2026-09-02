using DSAExperimentation.DataStructures.KeyedDisjointSet;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindAllPeopleWithSecret;

// LeetCode 2092. Find All People With Secret: secrets only propagate transitively
// within the SAME instant (a chain of meetings sharing one timestamp all resolve
// together), never across timestamps in a single hop. Meetings are grouped by time
// and processed in ascending order; for each timestamp group a fresh
// KeyedDisjointSet<int> is built over just that group's participants - the same
// "union on shared membership" shape AccountsMergeTests already uses for shared
// emails, applied here to shared timestamps instead. A component learns the secret
// for this timestamp iff at least one of its members already knew it walking in - a
// Set<int> tracks which component representatives qualify before a second pass marks
// every member of a qualifying component as now knowing the secret. Building a fresh
// KeyedDisjointSet per timestamp (rather than one global DisjointSet needing an
// "undo union" this repo has no primitive for) is what keeps a same-timestamp-only
// chain from ever leaking a false connection into a later timestamp's group.
public sealed partial class FindAllPeopleWithSecretTests
{
    [Fact]
    public void FindAllPeople_ClassicExample_ReturnsEveryoneReachableThroughTimeOrderedMeetings()
    {
        (int First, int Second, int Time)[] meetings = [(1, 2, 5), (2, 3, 8), (1, 5, 10)];

        var people = FindAllPeople(n: 6, meetings, firstPerson: 1);

        Assert.Equal([0, 1, 2, 3, 5], people);
    }

    [Fact]
    public void FindAllPeople_LaterTimestampCannotReachAnEarlierOnlyConnectedPerson_ExcludesThem()
    {
        (int First, int Second, int Time)[] meetings = [(3, 1, 3), (1, 2, 2), (0, 3, 3)];

        var people = FindAllPeople(n: 4, meetings, firstPerson: 3);

        Assert.Equal([0, 1, 3], people);
    }

    [Fact]
    public void FindAllPeople_SameTimestampChainPropagatesTransitively_ReturnsEveryone()
    {
        (int First, int Second, int Time)[] meetings = [(3, 4, 2), (1, 2, 1), (2, 3, 1)];

        var people = FindAllPeople(n: 5, meetings, firstPerson: 1);

        Assert.Equal([0, 1, 2, 3, 4], people);
    }

    private static int[] FindAllPeople(int n, (int First, int Second, int Time)[] meetings, int firstPerson)
    {
        var knowsSecret = new bool[n];
        knowsSecret[0] = true;
        knowsSecret[firstPerson] = true;

        foreach (var group in GroupByTimeAscending(meetings))
        {
            PropagateWithinTimestamp(group, knowsSecret);
        }

        var people = new List<int>();
        for (var person = 0; person < n; person++)
        {
            if (knowsSecret[person])
            {
                people.Add(person);
            }
        }

        return people.ToArray();
    }

    private static void PropagateWithinTimestamp((int First, int Second)[] group, bool[] knowsSecret)
    {
        var participants = CollectParticipants(group);
        var components = BuildComponents(group, participants);
        var secretRoots = FindSecretRoots(participants, components, knowsSecret);
        MarkQualifyingComponents(participants, components, secretRoots, knowsSecret);
    }

    private static List<int> CollectParticipants((int First, int Second)[] group)
    {
        var participants = new List<int>();
        foreach (var (first, second) in group)
        {
            participants.Add(first);
            participants.Add(second);
        }

        return participants;
    }

    private static KeyedDisjointSet<int> BuildComponents((int First, int Second)[] group, List<int> participants)
    {
        var components = new KeyedDisjointSet<int>(participants);

        foreach (var (first, second) in group)
        {
            components.TryUnion(first, second);
        }

        return components;
    }

    private static Set<int> FindSecretRoots(List<int> participants, KeyedDisjointSet<int> components, bool[] knowsSecret)
    {
        var secretRoots = new Set<int>();
        foreach (var person in participants)
        {
            if (knowsSecret[person] && components.TryFind(person, out var root))
            {
                secretRoots.TryAdd(root);
            }
        }

        return secretRoots;
    }

    private static void MarkQualifyingComponents(
        List<int> participants, KeyedDisjointSet<int> components, Set<int> secretRoots, bool[] knowsSecret)
    {
        foreach (var person in participants)
        {
            if (components.TryFind(person, out var root) && secretRoots.Has(root))
            {
                knowsSecret[person] = true;
            }
        }
    }

    private static IEnumerable<(int First, int Second)[]> GroupByTimeAscending((int First, int Second, int Time)[] meetings)
        => meetings
            .GroupBy(m => m.Time)
            .OrderBy(g => g.Key)
            .Select(g => g.Select(m => (m.First, m.Second)).ToArray());
}
