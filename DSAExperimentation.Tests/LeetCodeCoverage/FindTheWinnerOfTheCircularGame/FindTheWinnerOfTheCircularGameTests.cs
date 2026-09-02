using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheWinnerOfTheCircularGame;

// LeetCode 1823. Find the Winner of the Circular Game: this repo's own Queue<int>
// simulates the circle directly - every friend 1..n is Enqueued once, then each
// elimination round rotates k-1 friends from front to back (TryDequeue immediately
// re-Enqueued) before TryDequeue-ing and discarding the friend now at the front, who
// is exactly the kth friend counted from wherever the previous round left off. The
// last friend left in the queue is the winner.
public sealed partial class FindTheWinnerOfTheCircularGameTests
{
    [Fact]
    public void FindTheWinner_LeetCodeExampleOne_ReturnsThree()
    {
        var actual = FindTheWinner(n: 5, k: 2);
        Assert.Equal(3, actual);
    }

    [Fact]
    public void FindTheWinner_LeetCodeExampleTwo_ReturnsOne()
    {
        var actual = FindTheWinner(n: 6, k: 5);
        Assert.Equal(1, actual);
    }

    [Fact]
    public void FindTheWinner_SingleFriend_ReturnsThatFriend()
    {
        var actual = FindTheWinner(n: 1, k: 1);
        Assert.Equal(1, actual);
    }

    private static int FindTheWinner(int n, int k)
    {
        var friends = new RepoQueue();

        for (var friend = 1; friend <= n; friend++)
        {
            friends.Enqueue(friend);
        }

        while (friends.Count > 1)
        {
            for (var step = 0; step < k - 1; step++)
            {
                friends.TryDequeue(out var rotated);
                friends.Enqueue(rotated);
            }

            friends.TryDequeue(out _);
        }

        friends.TryDequeue(out var winner);
        return winner;
    }
}
