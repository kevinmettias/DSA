using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ProcessRestrictedFriendRequests;

// LeetCode 2076. Process Restricted Friend Requests: this repo's own DisjointSet.
// Each request is checked against every restriction using Find alone - no Union -
// before committing, so a rejected request never needs to be rolled back: Union(u,v)
// only ever runs once the request is already known safe.
public sealed partial class ProcessRestrictedFriendRequestsTests
{
    [Fact]
    public void FriendRequests_LeetCodeExample1_RejectsRequestThatWouldIndirectlyViolateRestriction()
    {
        int[][] restrictions = [[0, 1]];
        int[][] requests = [[0, 2], [2, 1]];

        var approved = FriendRequests(n: 3, restrictions, requests);

        Assert.Equal([true, false], approved);
    }

    [Fact]
    public void FriendRequests_LeetCodeExample2_RejectsRequestThatWouldViolateRestriction()
    {
        int[][] restrictions = [[0, 1]];
        int[][] requests = [[1, 2], [0, 2]];

        var approved = FriendRequests(n: 3, restrictions, requests);

        Assert.Equal([true, false], approved);
    }

    [Fact]
    public void FriendRequests_LeetCodeExample3_TracksRestrictionsAcrossMultipleRequests()
    {
        int[][] restrictions = [[0, 1], [1, 2], [2, 3]];
        int[][] requests = [[0, 4], [1, 2], [3, 1], [3, 4]];

        var approved = FriendRequests(n: 5, restrictions, requests);

        Assert.Equal([true, false, true, false], approved);
    }

    private static bool[] FriendRequests(int n, int[][] restrictions, int[][] requests)
    {
        var friends = new DisjointSet(n);
        var approved = new bool[requests.Length];

        for (var i = 0; i < requests.Length; i++)
        {
            approved[i] = TryApprove(friends, restrictions, requests[i][0], requests[i][1]);
        }

        return approved;
    }

    private static bool TryApprove(DisjointSet friends, int[][] restrictions, int person, int other)
    {
        var personRoot = friends.Find(person);
        var otherRoot = friends.Find(other);

        if (personRoot == otherRoot)
        {
            return true;
        }

        if (WouldViolateRestriction(friends, restrictions, personRoot, otherRoot))
        {
            return false;
        }

        friends.Union(personRoot, otherRoot);
        return true;
    }

    // Merging personRoot's and otherRoot's components newly connects a restricted
    // pair exactly when that pair's two current roots are (personRoot, otherRoot)
    // in either order - checked via Find alone, so a rejected merge never happened.
    private static bool WouldViolateRestriction(
        DisjointSet friends, int[][] restrictions, int personRoot, int otherRoot)
    {
        foreach (var restriction in restrictions)
        {
            var firstRoot = friends.Find(restriction[0]);
            var secondRoot = friends.Find(restriction[1]);

            var wouldConnect =
                (firstRoot == personRoot && secondRoot == otherRoot) ||
                (firstRoot == otherRoot && secondRoot == personRoot);

            if (wouldConnect)
            {
                return true;
            }
        }

        return false;
    }
}
