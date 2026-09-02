using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TheScoreOfStudentsSolvingMathExpression;

// LeetCode 2019. The Score of Students Solving Math Expression: interval DP over
// every way to fully parenthesize a left-to-right +/* expression - the classic
// "Different Ways to Add Parentheses" shape, memoized by this repo's own
// Memoizer<TState,TResult> keyed on (left,right) number-token indices. The
// per-interval achievable-value set is held in this repo's own HashMap<long,bool>
// rather than Set<long>: combining two sibling intervals needs to enumerate every
// achieved value, and Set<T> exposes only membership (Has/TryAdd), never
// enumeration, so HashMap's own Keys is the primitive that actually fits here.
public sealed class TheScoreOfStudentsSolvingMathExpressionTests
{
    // "2+3*4" has exactly two full parenthesizations: (2+3)*4 = 20 and 2+(3*4) = 14.
    // Standard operator precedence (multiplication before addition) evaluates to 14,
    // so that is the "correct" answer; 20 is the other achievable value; 99 is
    // achievable under no parenthesization at all.
    [Theory]
    [InlineData(14, 5)]
    [InlineData(20, 2)]
    [InlineData(99, 0)]
    public void Score_SimpleAddMulExpression_MatchesExpectedRule(int answer, int expected)
    {
        var actual = Score("2+3*4", answer);

        Assert.Equal(expected, actual);
    }

    private static int Score(string s, int answer)
    {
        var (numbers, ops) = Parse(s);

        if (answer == EvaluateWithPrecedence(numbers, ops))
        {
            return 5;
        }

        return AchievableValues(numbers, ops).HasKey(answer) ? 2 : 0;
    }

    private static (int[] Numbers, char[] Ops) Parse(string s)
    {
        var numbers = new List<int>();
        var ops = new List<char>();

        foreach (var c in s)
        {
            if (char.IsDigit(c))
            {
                numbers.Add(c - '0');
            }
            else
            {
                ops.Add(c);
            }
        }

        return (numbers.ToArray(), ops.ToArray());
    }

    // The one, unambiguous evaluation order a student who *did* follow precedence
    // would produce - a single left-to-right pass, no interval search needed.
    private static int EvaluateWithPrecedence(int[] numbers, char[] ops)
    {
        var sum = 0;
        var term = numbers[0];

        for (var i = 0; i < ops.Length; i++)
        {
            if (ops[i] == '*')
            {
                term *= numbers[i + 1];
            }
            else
            {
                sum += term;
                term = numbers[i + 1];
            }
        }

        return sum + term;
    }

    private static HashMap<long, bool> AchievableValues(int[] numbers, char[] ops)
        => Memoizer.Memoize<(int Left, int Right), HashMap<long, bool>>(
            (0, numbers.Length - 1),
            (range, solve) =>
            {
                var (left, right) = range;
                var values = new HashMap<long, bool>();

                if (left == right)
                {
                    values.Set(numbers[left], true);
                    return values;
                }

                for (var split = left; split < right; split++)
                {
                    var leftValues = solve((left, split));
                    var rightValues = solve((split + 1, right));

                    foreach (var a in leftValues.Keys)
                    {
                        foreach (var b in rightValues.Keys)
                        {
                            values.Set(ops[split] == '+' ? a + b : a * b, true);
                        }
                    }
                }

                return values;
            });
}
