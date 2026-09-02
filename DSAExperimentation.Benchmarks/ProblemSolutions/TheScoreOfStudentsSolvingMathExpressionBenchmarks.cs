using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// The Score of Students Solving Math Expression (LC 2019): plain recursion with no
// caching (every (left,right) sub-interval gets recomputed from scratch each time a
// different split path reaches it - the same overlapping-subproblems blowup matrix
// chain multiplication has) vs. this repo's own Memoizer<TState,TResult> caching by
// (left,right) number-token indices, with each interval's achievable-value set held
// in this repo's own HashMap<long,bool> (Set<long> exposes no enumeration, so it
// cannot support the pairwise combine step - see the coverage test's own note).
[MemoryDiagnoser]
public class TheScoreOfStudentsSolvingMathExpressionBenchmarks
{
    private const int MaxDigitValueExclusive = 9;
    private const int OperatorChoiceCount = 2;

    [Params(6, 10)]
    public int NumberCount;

    private int[] _numbers = null!;
    private char[] _ops = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _numbers = Enumerable.Range(0, NumberCount).Select(_ => random.Next(1, MaxDigitValueExclusive)).ToArray();
        _ops = Enumerable.Range(0, NumberCount - 1).Select(_ => random.Next(0, OperatorChoiceCount) == 0 ? '+' : '*').ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RecursiveNoMemo()
    {
        var values = Solve(0, _numbers.Length - 1);
        return new HashSet<long>(values).Count;
    }

    private List<long> Solve(int left, int right)
    {
        if (left == right)
        {
            return [_numbers[left]];
        }

        var results = new List<long>();

        for (var split = left; split < right; split++)
        {
            var leftValues = Solve(left, split);
            var rightValues = Solve(split + 1, right);

            foreach (var a in leftValues)
            {
                foreach (var b in rightValues)
                {
                    results.Add(_ops[split] == '+' ? a + b : a * b);
                }
            }
        }

        return results;
    }

    [Benchmark]
    public int MemoizedHashMap()
    {
        var values = Memoizer.Memoize<(int Left, int Right), HashMap<long, bool>>(
            (0, _numbers.Length - 1),
            (range, solve) =>
            {
                var (left, right) = range;
                var map = new HashMap<long, bool>();

                if (left == right)
                {
                    map.Set(_numbers[left], true);
                    return map;
                }

                for (var split = left; split < right; split++)
                {
                    var leftValues = solve((left, split));
                    var rightValues = solve((split + 1, right));

                    foreach (var a in leftValues.Keys)
                    {
                        foreach (var b in rightValues.Keys)
                        {
                            map.Set(_ops[split] == '+' ? a + b : a * b, true);
                        }
                    }
                }

                return map;
            });

        return values.Count;
    }
}
