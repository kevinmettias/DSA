using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.LoudAndRich;

// LeetCode 851. Loud and Rich: richer[i] = [a, b] means a definitely has more
// money than b, so answer[x] is the least quiet person among everyone who has at
// least as much money as x (x included).
//
// Turning each pair into a richer -> poorer edge makes the relation a DAG, and
// the two strategies differ only in how they exploit it: the baseline re-walks
// every person's own "definitely richer than" reachable set from scratch, while
// the composed solution orders everyone richest-first with this repo's own
// Kahn's-algorithm TopologicalSort.TrySort and threads the best answer so far
// down each edge in one linear pass.
internal static class LoudAndRichSolution
{
    // This repo's own topological sort: TrySort orders every person richest-first
    // (in-degree 0 means "nobody confirmed richer yet"), then one linear DP pass
    // pushes each person's current best answer onto everyone they are richer
    // than, O(V+E) total.
    public static int[] QuietestByTopologicalDpPass(int[][] richer, int[] quiet) =>
        QuietestByTopologicalDpPass(BuildPeople(richer, quiet.Length), quiet);

    public static int[] QuietestByTopologicalDpPass(List<PersonNode> people, int[] quiet)
    {
        TopologicalSort.TrySort<
            PersonNode, PersonTopology, ListChildren<PersonNode>,
            NaturalChildOrder<PersonNode, ListChildren<PersonNode>>, ListChildren<PersonNode>>(
            people, out var ordering);

        var answer = Enumerable.Range(0, people.Count).ToArray();

        foreach (var node in ordering)
        {
            foreach (var poorer in node.Poorer)
            {
                if (quiet[answer[node.Id]] < quiet[answer[poorer.Id]])
                {
                    answer[poorer.Id] = answer[node.Id];
                }
            }
        }

        return answer;
    }

    // The textbook answer: for each person run a fresh depth-first walk over the
    // reversed "is richer than" edges and take the quietest person it reaches,
    // O(n * (V+E)) total because a wealthy person sits inside many other
    // people's walks. Deliberately written with a BCL Stack and HashSet - it is
    // the arm the composed solution above has to justify itself against.
    public static int[] QuietestByPerPersonWalk(int[][] richer, int[] quiet) =>
        QuietestByPerPersonWalk(BuildPeople(richer, quiet.Length), quiet);

    public static int[] QuietestByPerPersonWalk(List<PersonNode> people, int[] quiet)
    {
        var answer = new int[people.Count];

        for (var i = 0; i < people.Count; i++)
        {
            answer[i] = FindQuietestRicher(people[i], quiet);
        }

        return answer;
    }

    private static int FindQuietestRicher(PersonNode start, int[] quiet)
    {
        var visited = new HashSet<PersonNode> { start };
        var stack = new Stack<PersonNode>();
        stack.Push(start);

        var quietest = start.Id;

        while (stack.Count > 0)
        {
            var node = stack.Pop();

            if (quiet[node.Id] < quiet[quietest])
            {
                quietest = node.Id;
            }

            foreach (var richer in node.Richer)
            {
                if (visited.Add(richer))
                {
                    stack.Push(richer);
                }
            }
        }

        return quietest;
    }

    private static List<PersonNode> BuildPeople(int[][] richer, int personCount)
    {
        var people = new List<PersonNode>(personCount);

        for (var id = 0; id < personCount; id++)
        {
            people.Add(new PersonNode(id));
        }

        foreach (var pair in richer)
        {
            var wealthier = pair[0];
            var poorer = pair[1];

            people[wealthier].Poorer.Add(people[poorer]);
            people[poorer].Richer.Add(people[wealthier]);
        }

        return people;
    }
}
