using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Compatibility Score Sum (LC 1947): the textbook unmemoized bitmask
// recursion over (student, usedMentorMask) - the same state re-explored from
// scratch down every branch, since many different pick orders reach the identical
// "these mentors are already taken" state - vs. the same recursion routed through
// this repo's own Memoizer, the identical (int, int) tuple-state shape
// MinimumCostToConnectTwoGroupsOfPointsBenchmarks already uses for LC 1595's
// (index, mask) recursion.
[MemoryDiagnoser]
public class MaximumCompatibilityScoreSumBenchmarks
{
    private const int QuestionCount = 8;
    private const int AnswerOptionCount = 2;

    // LC problem number, reused as the deterministic answer-matrix seed.
    private const int RandomSeed = 1947;

    [Params(4, 7)]
    public int GroupSize;

    private int[][] _score = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var (students, mentors) = GenerateAnswers(random);
        _score = ComputeCompatibilityScores(students, mentors);
    }

    private (int[][] Students, int[][] Mentors) GenerateAnswers(Random random)
    {
        var students = new int[GroupSize][];
        var mentors = new int[GroupSize][];

        for (var i = 0; i < GroupSize; i++)
        {
            students[i] = Enumerable.Range(0, QuestionCount).Select(_ => random.Next(0, AnswerOptionCount)).ToArray();
            mentors[i] = Enumerable.Range(0, QuestionCount).Select(_ => random.Next(0, AnswerOptionCount)).ToArray();
        }

        return (students, mentors);
    }

    private int[][] ComputeCompatibilityScores(int[][] students, int[][] mentors)
    {
        var score = new int[GroupSize][];

        for (var i = 0; i < GroupSize; i++)
        {
            score[i] = new int[GroupSize];
            for (var j = 0; j < GroupSize; j++)
            {
                score[i][j] = CountMatches(students[i], mentors[j]);
            }
        }

        return score;
    }

    private static int CountMatches(int[] studentAnswers, int[] mentorAnswers)
    {
        var matches = 0;

        for (var k = 0; k < QuestionCount; k++)
        {
            if (studentAnswers[k] == mentorAnswers[k])
            {
                matches++;
            }
        }

        return matches;
    }

    [Benchmark(Baseline = true)]
    public int BruteForceRecursion() => Best(0, 0);

    private int Best(int student, int usedMask)
    {
        if (student == GroupSize)
        {
            return 0;
        }

        var top = int.MinValue;
        for (var mentor = 0; mentor < GroupSize; mentor++)
        {
            if ((usedMask & (1 << mentor)) != 0)
            {
                continue;
            }

            var candidate = _score[student][mentor] + Best(student + 1, usedMask | (1 << mentor));
            top = Math.Max(top, candidate);
        }

        return top;
    }

    [Benchmark]
    public int MemoizedRecursion() => Memoizer.Memoize<(int Student, int UsedMask), int>((0, 0), (state, best) =>
    {
        var (student, usedMask) = state;

        if (student == GroupSize)
        {
            return 0;
        }

        var top = int.MinValue;
        for (var mentor = 0; mentor < GroupSize; mentor++)
        {
            if ((usedMask & (1 << mentor)) != 0)
            {
                continue;
            }

            var candidate = _score[student][mentor] + best((student + 1, usedMask | (1 << mentor)));
            top = Math.Max(top, candidate);
        }

        return top;
    });
}
