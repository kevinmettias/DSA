using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Loud and Rich (LC 851): NaivePerPersonWalk re-walks each person's own
// "definitely richer than" reachable set from scratch with a fresh DFS, O(n *
// (V+E)) total, since a busy person can sit inside many other people's walks.
// TopologicalDpPass instead orders every person richest-first with this repo's
// own Kahn's-algorithm TopologicalSort.TrySort, then threads the quietest
// person seen so far down each "poorer than" edge in one linear pass, O(V+E)
// total - the canonical solution to this problem. Richer edges point from a
// lower id to a higher one (capped fan-out) so the relation is a
// guaranteed-acyclic DAG, the same generation shape CourseScheduleIIBenchmarks
// already uses.
[MemoryDiagnoser]
public class LoudAndRichBenchmarks
{
    [Params(50, 1_000)]
    public int PersonCount;

    private int[] _quiet = null!;
    private List<PersonNode> _people = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(5);
        _quiet = Enumerable.Range(0, PersonCount).Select(_ => random.Next(0, PersonCount)).ToArray();
        _people = Enumerable.Range(0, PersonCount).Select(id => new PersonNode(id)).ToList();

        for (var i = 0; i < PersonCount; i++)
        {
            var fanOut = Math.Min(3, PersonCount - 1 - i);
            for (var f = 1; f <= fanOut; f++)
            {
                // person i is richer than person i + f.
                _people[i].Poorer.Add(_people[i + f]);
                _people[i + f].Richer.Add(_people[i]);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int NaivePerPersonWalk()
    {
        var quietest = 0;

        for (var i = 0; i < PersonCount; i++)
        {
            quietest = FindQuietestReachable(_people[i]);
        }

        return quietest;
    }

    [Benchmark]
    public int TopologicalDpPass()
    {
        TopologicalSort.TrySort<
            PersonNode, PersonTopology, ListChildren<PersonNode>,
            NaturalChildOrder<PersonNode, ListChildren<PersonNode>>, ListChildren<PersonNode>>(
            _people, out var ordering);

        var answer = Enumerable.Range(0, PersonCount).ToArray();

        foreach (var node in ordering)
        {
            foreach (var poorer in node.Poorer)
            {
                if (_quiet[answer[node.Id]] < _quiet[answer[poorer.Id]])
                {
                    answer[poorer.Id] = answer[node.Id];
                }
            }
        }

        return answer[0];
    }

    private int FindQuietestReachable(PersonNode start)
    {
        var visited = new HashSet<PersonNode> { start };
        var stack = new Stack<PersonNode>();
        stack.Push(start);
        var quietest = start.Id;

        while (stack.Count > 0)
        {
            var node = stack.Pop();
            if (_quiet[node.Id] < _quiet[quietest])
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

    // See LoudAndRichTests.Fixtures for the full explanation - repeated here
    // rather than shared because TwoSumBenchmarks/MedianOfTwoSortedArraysBenchmarks
    // establish this project keeps its own copy of the solution rather than
    // depending on the Tests project.
    private sealed class PersonNode(int id)
    {
        public int Id { get; } = id;

        public List<PersonNode> Poorer { get; } = [];

        public List<PersonNode> Richer { get; } = [];
    }

    private readonly struct PersonTopology : IGraphTopology<PersonNode, ListChildren<PersonNode>>
    {
        public static ListChildren<PersonNode> GetChildren(PersonNode node) => new(node.Poorer);
    }
}
