using DSAExperimentation.DataStructures.Set;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<string>;
using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.LeetCode.RemoveInvalidParentheses;

// LeetCode 301. Remove Invalid Parentheses: report every string reachable from s
// by removing the minimum number of parentheses that makes it valid.
//
// RemoveByBruteForceAllSubsets is the textbook answer most people reach for
// first - enumerate every one of the 2^n character subsets and keep the valid
// ones of maximum length. RemoveByQueueBfs is this repo's own level-by-level
// BFS: Queue<string> as the frontier, Set<string> to dedupe candidates already
// tried, and Stack<char> (the same LC20 ValidParentheses bracket-matching check)
// to test each candidate's validity. The first level containing any valid
// candidate holds every result at the minimum removal count, so the search never
// explores more removals than necessary.
internal static class RemoveInvalidParenthesesSolution
{
    // Enumerates every subset of s's characters, keeping the valid ones of
    // maximum length. Deliberately written without this repo's primitives - it
    // is the arm the composed BFS below has to justify itself against.
    //
    // The original benchmark arm this was extracted from tracked only the
    // winning length as an int, never the strings themselves - promoted here to
    // LeetCode's real answer shape (every result at that length) since nothing
    // about the enumeration changes to do so.
    public static List<string> RemoveByBruteForceAllSubsets(string s)
    {
        var maxLength = -1;
        var results = new List<string>();
        var totalMasks = 1 << s.Length;

        for (var mask = 0; mask < totalMasks; mask++)
        {
            maxLength = ConsiderSubset(s, mask, maxLength, results);
        }

        return results;
    }

    // Keeps the results list in step with the longest valid subset seen so far:
    // a longer one replaces everything collected at a shorter length, an equal
    // one is added only if this exact string is not already in there. Returns the
    // - possibly raised - best length, which is the only state the caller keeps.
    private static int ConsiderSubset(string s, int mask, int maxLength, List<string> results)
    {
        var chars = BuildSubset(s, mask);

        if (chars.Count < maxLength || !IsValidSubset(chars))
        {
            return maxLength;
        }

        if (chars.Count > maxLength)
        {
            results.Clear();
            maxLength = chars.Count;
        }

        var candidate = new string(chars.ToArray());

        if (!results.Contains(candidate))
        {
            results.Add(candidate);
        }

        return maxLength;
    }

    // The subset mask selects only the bits that stand for a kept character, so
    // the kept characters come out in their original order.
    private static List<char> BuildSubset(string s, int mask)
    {
        var chars = new List<char>();

        for (var i = 0; i < s.Length; i++)
        {
            if ((mask & (1 << i)) != 0)
            {
                chars.Add(s[i]);
            }
        }

        return chars;
    }

    private static bool IsValidSubset(List<char> chars)
    {
        var balance = 0;

        foreach (var ch in chars)
        {
            if (ch == '(')
            {
                balance++;
            }
            else if (ch == ')')
            {
                balance--;

                if (balance < 0)
                {
                    return false;
                }
            }
        }

        return balance == 0;
    }

    public static List<string> RemoveByQueueBfs(string s)
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
