using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Tests.LeetCodeCoverage.JumpGameVII;

// LeetCode 1871. Jump Game VII: reachability over an implicit graph where index i
// (only when s[i] == '0') has an edge to every j in [i+minJump, min(i+maxJump,
// n-1)] with s[j] == '0' - the same "board too large/awkward to materialize as
// actual edges" shape EscapeALargeMazeTests already uses this repo's own
// DepthFirstSearch.Traverse for, just over a 1-D index range instead of a 2-D grid.
// Traverse's own HashSet-backed visited tracking is exactly what turns this into a
// single O(n * (maxJump-minJump)) walk instead of the exponential blowup an
// unmemoized recursive search would hit when multiple jump chains reconverge on the
// same index (see JumpGameVIIBenchmarks' UnmemoizedRecursiveSearch baseline).
public sealed class JumpGameVIITests
{
    [Fact]
    public void CanReach_LeetCodeExampleOne_ReturnsTrue()
    {
        var canReach = CanReach("011010", minJump: 2, maxJump: 3);
        Assert.True(canReach);
    }

    [Fact]
    public void CanReach_LeetCodeExampleTwo_ReturnsFalse()
    {
        var canReach = CanReach("01101110", minJump: 2, maxJump: 3);
        Assert.False(canReach);
    }

    [Fact]
    public void CanReach_SingleZeroTwoCharacterString_ReachesItselfTrivially()
    {
        var canReach = CanReach("00", minJump: 1, maxJump: 1);
        Assert.True(canReach);
    }

    private static bool CanReach(string s, int minJump, int maxJump)
    {
        var successors = Successors(s, minJump, maxJump);
        var reached = DepthFirstSearch.Traverse(0, successors);
        return reached.Contains(s.Length - 1);
    }

    private static Func<int, IEnumerable<int>> Successors(string s, int minJump, int maxJump)
        => i =>
        {
            var lastIndex = Math.Min(i + maxJump, s.Length - 1);
            var next = new List<int>();

            for (var j = i + minJump; j <= lastIndex; j++)
            {
                if (s[j] == '0')
                {
                    next.Add(j);
                }
            }

            return next;
        };
}
