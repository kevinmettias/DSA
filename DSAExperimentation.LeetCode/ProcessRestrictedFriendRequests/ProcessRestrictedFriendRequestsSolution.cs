using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.ProcessRestrictedFriendRequests;

// LeetCode 2076. Process Restricted Friend Requests: n people, a list of pairs who
// must never end up in the same friend group (directly or indirectly), and a stream
// of friend requests processed in order. A request is approved when granting it
// would not connect a restricted pair; approved requests merge the two groups, and
// rejected ones change nothing.
//
// Both strategies answer the same question - "would merging these two groups newly
// connect a restricted pair?" - and differ only in how group membership is tracked
// between requests.
internal static class ProcessRestrictedFriendRequestsSolution
{
    // The textbook answer: keep an adjacency list of the friendships approved so far
    // and re-derive each person's group with a fresh DFS on every request, an
    // O(n + e) walk per request. Deliberately written without this repo's primitives
    // - it is the arm the composed strategy below has to justify itself against.
    public static bool[] FriendRequestsByReachabilityScan(int n, int[][] restrictions, int[][] requests)
    {
        var adjacency = new List<int>[n];

        for (var i = 0; i < n; i++)
        {
            adjacency[i] = [];
        }

        var approved = new bool[requests.Length];

        for (var i = 0; i < requests.Length; i++)
        {
            approved[i] = TryApproveByReachability(adjacency, restrictions, requests[i][0], requests[i][1]);
        }

        return approved;
    }

    private static bool TryApproveByReachability(
        List<int>[] adjacency, int[][] restrictions, int person, int other)
    {
        var personGroup = ReachableSet(adjacency, person);

        if (personGroup.Contains(other))
        {
            return true;
        }

        var otherGroup = ReachableSet(adjacency, other);

        if (GroupsWouldViolateRestriction(restrictions, personGroup, otherGroup))
        {
            return false;
        }

        adjacency[person].Add(other);
        adjacency[other].Add(person);
        return true;
    }

    private static bool GroupsWouldViolateRestriction(
        int[][] restrictions, HashSet<int> personGroup, HashSet<int> otherGroup)
    {
        foreach (var restriction in restrictions)
        {
            var first = restriction[0];
            var second = restriction[1];

            var wouldConnect =
                (personGroup.Contains(first) && otherGroup.Contains(second)) ||
                (personGroup.Contains(second) && otherGroup.Contains(first));

            if (wouldConnect)
            {
                return true;
            }
        }

        return false;
    }

    private static HashSet<int> ReachableSet(List<int>[] adjacency, int start)
    {
        var visited = new HashSet<int> { start };
        var stack = new Stack<int>();
        stack.Push(start);

        while (stack.Count > 0)
        {
            var node = stack.Pop();

            foreach (var next in adjacency[node])
            {
                if (visited.Add(next))
                {
                    stack.Push(next);
                }
            }
        }

        return visited;
    }

    // This repo's own DisjointSet: each request is checked against every restriction
    // using Find alone - no Union - before committing, so a rejected request never
    // needs to be rolled back, and Find is near O(1) amortized instead of a fresh
    // O(n + e) walk per request.
    public static bool[] FriendRequestsByDisjointSet(int n, int[][] restrictions, int[][] requests)
    {
        var friends = new DisjointSet(n);
        var approved = new bool[requests.Length];

        for (var i = 0; i < requests.Length; i++)
        {
            approved[i] = TryApproveByUnion(friends, restrictions, requests[i][0], requests[i][1]);
        }

        return approved;
    }

    private static bool TryApproveByUnion(
        DisjointSet friends, int[][] restrictions, int person, int other)
    {
        var personRoot = friends.Find(person);
        var otherRoot = friends.Find(other);

        if (personRoot == otherRoot)
        {
            return true;
        }

        if (RootsWouldViolateRestriction(friends, restrictions, personRoot, otherRoot))
        {
            return false;
        }

        friends.Union(personRoot, otherRoot);
        return true;
    }

    // Merging personRoot's and otherRoot's components newly connects a restricted
    // pair exactly when that pair's two current roots are (personRoot, otherRoot)
    // in either order - checked via Find alone, so a rejected merge never happened.
    private static bool RootsWouldViolateRestriction(
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
