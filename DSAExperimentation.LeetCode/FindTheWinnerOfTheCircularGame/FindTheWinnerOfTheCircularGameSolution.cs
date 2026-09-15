using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.LeetCode.FindTheWinnerOfTheCircularGame;

// LeetCode 1823. Find the Winner of the Circular Game: friends 1..n sit in a
// circle, counting k starting from friend 1 eliminates whoever the count lands on,
// and counting resumes from the friend after them until one is left.
//
// Both strategies simulate the circle rather than solve the Josephus recurrence;
// they differ in how "the circle after an elimination" is represented. A List<int>
// keeps the survivors contiguous and pays an O(n) shift on every RemoveAt, so its
// cost is O(n^2) regardless of k. This repo's own Queue<int> instead rotates the
// k-1 friends that are merely counted past from front to back, which makes the
// elimination itself O(1) and the total O(n * k) - the faster arm whenever k is
// small relative to n, the common case this problem's constraints allow.
internal static class FindTheWinnerOfTheCircularGameSolution
{
    // The textbook answer: hold the survivors in a BCL List and delete the losers
    // in place, advancing the count index modulo the shrinking circle. Deliberately
    // written without this repo's primitives - it is the arm the queue rotation
    // below has to justify itself against.
    public static int FindTheWinnerByListRemoval(int n, int k)
    {
        var friends = BuildFriendCircle(n);

        return EliminateUntilOneRemains(friends, k);
    }

    // Friends 1..n seated in order, so a survivor's position in the list is its
    // position in the circle.
    private static List<int> BuildFriendCircle(int n)
    {
        var friends = new List<int>(n);

        for (var friend = 1; friend <= n; friend++)
        {
            friends.Add(friend);
        }

        return friends;
    }

    // Delete the loser in place and advance the count index modulo the shrinking
    // circle; the last friend left standing is the winner.
    private static int EliminateUntilOneRemains(List<int> friends, int k)
    {
        var index = 0;

        while (friends.Count > 1)
        {
            index = (index + k - 1) % friends.Count;
            friends.RemoveAt(index);
        }

        return friends[0];
    }

    // Every friend is Enqueued once, then each round rotates k-1 friends from front
    // to back (TryDequeue immediately re-Enqueued) before TryDequeue-ing and
    // discarding the friend now at the front - exactly the kth friend counted from
    // wherever the previous round left off, with no re-indexing of the survivors.
    // The last friend left in the queue is the winner.
    public static int FindTheWinnerByQueueRotation(int n, int k)
    {
        var friends = new RepoQueue();

        for (var friend = 1; friend <= n; friend++)
        {
            friends.Enqueue(friend);
        }

        while (friends.Count > 1)
        {
            RotatePastCountedFriends(friends, k);
            friends.TryDequeue(out _);
        }

        friends.TryDequeue(out var winner);
        return winner;
    }

    private static void RotatePastCountedFriends(RepoQueue friends, int k)
    {
        for (var step = 0; step < k - 1; step++)
        {
            friends.TryDequeue(out var rotated);
            friends.Enqueue(rotated);
        }
    }
}
