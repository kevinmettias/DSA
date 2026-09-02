using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Number of Groups Getting Fresh Donuts (LC 1815): the textbook brute force
// tries every permutation of groups (n! orderings, each O(n) to score) vs. this
// repo's own Memoizer (Algorithms/DynamicProgramming/Memoizer.cs) driving a search
// over (running residue, remaining remainder counts) state instead - a vastly
// smaller state space than n! once multiple serving orders collapse onto the same
// state (MaximumNumberOfGroupsGettingFreshDonutsTests precedent).
[MemoryDiagnoser]
public class MaximumNumberOfGroupsGettingFreshDonutsBenchmarks
{
    private const int BatchSize = 5;
    private const int MaxGroupSizeExclusive = 50;
    private const string StateSeparator = "|";

    [Params(6, 9)]
    public int GroupCount;

    private int[] _groups = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _groups = Enumerable.Range(0, GroupCount).Select(_ => random.Next(1, MaxGroupSizeExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int AllPermutations() => BestOverAllPermutations(_groups);

    private static int BestOverAllPermutations(int[] groups)
    {
        var best = 0;
        Permute(groups, 0, ref best);
        return best;
    }

    private static void Permute(int[] groups, int start, ref int best)
    {
        if (start == groups.Length)
        {
            best = Math.Max(best, CountNice(groups));
            return;
        }

        for (var i = start; i < groups.Length; i++)
        {
            (groups[start], groups[i]) = (groups[i], groups[start]);
            Permute(groups, start + 1, ref best);
            (groups[start], groups[i]) = (groups[i], groups[start]);
        }
    }

    private static int CountNice(int[] order)
    {
        var running = 0;
        var nice = 0;

        foreach (var size in order)
        {
            if (running % BatchSize == 0)
            {
                nice++;
            }

            running += size;
        }

        return nice;
    }

    [Benchmark]
    public int MemoizedSearch() => MaxHappyGroups(BatchSize, _groups);

    private static int MaxHappyGroups(int batchSize, int[] groups)
    {
        var counts = new int[batchSize];
        foreach (var g in groups)
        {
            counts[g % batchSize]++;
        }

        var niceFromZeroRemainder = counts[0];
        var remainderCounts = counts.Skip(1).ToArray();

        if (remainderCounts.All(c => c == 0))
        {
            return niceFromZeroRemainder;
        }

        var initialState = EncodeState(0, remainderCounts);
        var extraNice = Memoizer.Memoize<string, int>(initialState, (state, best) => Solve(state, best, batchSize));

        return niceFromZeroRemainder + extraNice;
    }

    private static int Solve(string state, Func<string, int> best, int batchSize)
    {
        var decoded = DecodeState(state);

        if (Array.TrueForAll(decoded.Counts, c => c == 0))
        {
            return 0;
        }

        var result = 0;

        for (var i = 0; i < decoded.Counts.Length; i++)
        {
            if (decoded.Counts[i] == 0)
            {
                continue;
            }

            var score = ScoreForTakingRemainder(i, decoded, batchSize, best);
            result = Math.Max(result, score);
        }

        return result;
    }

    private static int ScoreForTakingRemainder(int index, (int Residue, int[] Counts) decoded, int batchSize, Func<string, int> best)
    {
        var remainder = index + 1;
        var nice = decoded.Residue == 0 ? 1 : 0;
        var nextCounts = (int[])decoded.Counts.Clone();
        nextCounts[index]--;
        var nextResidue = (decoded.Residue + remainder) % batchSize;
        var nextState = EncodeState(nextResidue, nextCounts);

        return nice + best(nextState);
    }

    private static string EncodeState(int residue, int[] counts) => residue + StateSeparator + string.Join(',', counts);

    private static (int Residue, int[] Counts) DecodeState(string state)
    {
        var parts = state.Split('|');
        var counts = Array.ConvertAll(parts[1].Split(','), int.Parse);
        return (int.Parse(parts[0]), counts);
    }
}
