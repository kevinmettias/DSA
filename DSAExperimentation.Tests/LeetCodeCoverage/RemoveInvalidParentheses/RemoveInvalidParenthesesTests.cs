using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<string>;
using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<char>;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveInvalidParentheses;

// LeetCode 301. Remove Invalid Parentheses: level-by-level BFS over "remove one
// paren" candidates, using this repo's own Queue<string> as the frontier, Set<string>
// to dedupe candidates already tried, and Stack<char> (the same ValidParentheses/LC20
// bracket-matching check) to test each candidate's validity. The first level
// containing any valid candidate holds every result at the minimum removal count.
public sealed partial class RemoveInvalidParenthesesTests
{
    [Fact]
    public void Remove_ExtraClosingParens_ReturnsBothMinimalRemovals()
        => Assert.Equal(new[] { "(())()", "()()()" }.Order(), Remove("()())()").Order());

    [Fact]
    public void Remove_LettersInterspersed_PreservesLettersAndRemovesOnlyParens()
        => Assert.Equal(new[] { "(a())()", "(a)()()" }.Order(), Remove("(a)())()").Order());

    [Fact]
    public void Remove_AllInvalid_ReturnsEmptyString()
        => Assert.Equal(new[] { "" }, Remove(")("));

    private static List<string> Remove(string s)
    {
        var visited = new Set<string>();
        var queue = new RepoQueue();
        visited.TryAdd(s);
        queue.Enqueue(s);

        while (queue.Count > 0)
        {
            var validAtThisLevel = ProcessLevel(queue, visited);

            if (validAtThisLevel.Count > 0)
            {
                return validAtThisLevel;
            }
        }

        return [];
    }

    private static List<string> ProcessLevel(RepoQueue queue, Set<string> visited)
    {
        var levelSize = queue.Count;
        var validAtThisLevel = new List<string>();

        for (var i = 0; i < levelSize; i++)
        {
            queue.TryDequeue(out var candidate);
            ProcessCandidate(candidate, queue, visited, validAtThisLevel);
        }

        return validAtThisLevel;
    }

    private static void ProcessCandidate(string candidate, RepoQueue queue, Set<string> visited, List<string> validAtThisLevel)
    {
        if (IsValid(candidate))
        {
            validAtThisLevel.Add(candidate);
            return;
        }

        for (var j = 0; j < candidate.Length; j++)
        {
            if (candidate[j] != '(' && candidate[j] != ')')
            {
                continue;
            }

            var next = candidate.Remove(j, 1);

            if (visited.TryAdd(next))
            {
                queue.Enqueue(next);
            }
        }
    }

    private static bool IsValid(string s)
    {
        var openers = new RepoStack();

        foreach (var ch in s)
        {
            if (ch == '(')
            {
                openers.Push(ch);
            }
            else if (ch == ')' && !openers.TryPop(out _))
            {
                return false;
            }
        }

        return openers.Count == 0;
    }
}
