using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.TheScoreOfStudentsSolvingMathExpression;

// LeetCode 2019. The Score of Students Solving Math Expression: grade a class of
// answers to a left-to-right +/* expression and return the total - 5 points for the
// value standard operator precedence produces, 2 points for any value SOME full
// parenthesization produces, 0 otherwise.
//
// Grading needs two facts about the expression: its one correct value (a single
// left-to-right pass, no search at all) and the set of values achievable under any
// parenthesization - the classic "Different Ways to Add Parentheses" interval shape,
// which is where all the cost is. The two strategies below differ only in how that
// set is built: plain split/recurse/combine, which re-derives a sub-interval every
// time a different split path reaches it, or the same recurrence over this repo's
// own Memoizer<TState,TResult> keyed on (left, right) number-token indices.
//
// The memoized arm holds each interval's achievable values in this repo's own
// HashMap<long,bool> rather than Set<long>: combining two sibling intervals has to
// ENUMERATE every value the children achieved, and Set<T> exposes only membership
// (Has/TryAdd), never enumeration, so HashMap's own Keys is the primitive that
// actually fits here.
//
// Neither arm caps interval values at LeetCode's answer ceiling. The published
// constraint (0 <= answers[i] <= 1000) makes that pruning safe and it is what a
// contest submission would do, but it changes the search this pair exists to
// compare, so both strategies enumerate the full achievable set.
internal static class TheScoreOfStudentsSolvingMathExpressionSolution
{
    // LeetCode's own grading scale for this problem.
    private const int CorrectAnswerScore = 5;
    private const int AchievableAnswerScore = 2;
    private const int WrongAnswerScore = 0;

    private const char Addition = '+';
    private const char Multiplication = '*';
    private const char ZeroDigit = '0';

    // The textbook answer: split the interval at every operator, recurse into both
    // halves and combine each (left, right) pair, re-deriving any sub-interval that
    // more than one split path reaches. Deliberately written with nothing but BCL
    // containers and the call stack - it is the arm the memoized strategy below has
    // to justify itself against.
    public static int ScoreOfStudentsByUnmemoizedRecursion(string expression, int[] answers)
    {
        var (numbers, ops) = Parse(expression);
        var correct = EvaluateWithPrecedence(numbers, ops);
        var achieved = AchievedValues(numbers, ops, 0, numbers.Length - 1);

        return TotalScore(answers, correct, new AchievableSetInHashSet(achieved));
    }

    // The same recurrence driven through this repo's own Memoizer, keyed on the
    // (left, right) number-token indices, so each interval is solved once and its
    // achievable-value map shared by every split path that reaches it.
    public static int ScoreOfStudentsByMemoizedIntervals(string expression, int[] answers)
    {
        var (numbers, ops) = Parse(expression);
        var correct = EvaluateWithPrecedence(numbers, ops);
        var achievable = AchievableValues(numbers, ops);

        return TotalScore(answers, correct, new AchievableSetInHashMap(achievable));
    }

    private static HashMap<long, bool> AchievableValues(int[] numbers, char[] ops)
        => Memoizer.Memoize<(int Left, int Right), HashMap<long, bool>>(
            (0, numbers.Length - 1),
            new ValuesOverIntervals(numbers, ops));

    // The recurrence, as a named type: the achievable-value set for one (Left, Right)
    // interval. The two token streams it reads arrive through the primary constructor
    // and the memoized continuation through `rest`.
    private sealed class ValuesOverIntervals(int[] numbers, char[] ops)
        : IRecurrence<(int Left, int Right), HashMap<long, bool>>
    {
        public HashMap<long, bool> Replay(
            (int Left, int Right) range,
            IRecurrence<(int Left, int Right), HashMap<long, bool>> rest)
        {
            var (left, right) = range;

            if (left == right)
            {
                return SingleValue(numbers[left]);
            }

            return ValuesOverSplits(left, right, rest);
        }

        private HashMap<long, bool> ValuesOverSplits(
            int left, int right, IRecurrence<(int Left, int Right), HashMap<long, bool>> rest)
        {
            var values = new HashMap<long, bool>();

            for (var split = left; split < right; split++)
            {
                var leftValues = rest.Replay((left, split), rest);
                var rightValues = rest.Replay((split + 1, right), rest);
                SetCombinations(ops[split], leftValues, rightValues, values);
            }

            return values;
        }
    }

    private static HashMap<long, bool> SingleValue(long value)
    {
        var values = new HashMap<long, bool>();
        values.Set(value, true);

        return values;
    }

    private static void SetCombinations(
        char op,
        HashMap<long, bool> leftValues,
        HashMap<long, bool> rightValues,
        HashMap<long, bool> values)
    {
        foreach (var a in leftValues.Keys)
        {
            foreach (var b in rightValues.Keys)
            {
                var combined = Combine(op, a, b);
                values.Set(combined, true);
            }
        }
    }

    // The expression is single digits separated by single-character operators, so
    // one pass splits it into the two token streams both strategies index into.
    private static (int[] Numbers, char[] Ops) Parse(string expression)
    {
        var numbers = new List<int>();
        var ops = new List<char>();

        foreach (var c in expression)
        {
            if (char.IsDigit(c))
            {
                numbers.Add(c - ZeroDigit);
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
            if (ops[i] == Multiplication)
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

    private static List<long> AchievedValues(int[] numbers, char[] ops, int left, int right)
    {
        if (left == right)
        {
            return [numbers[left]];
        }

        return AchievedValuesOverSplits(numbers, ops, left, right);
    }

    private static List<long> AchievedValuesOverSplits(int[] numbers, char[] ops, int left, int right)
    {
        var values = new List<long>();

        for (var split = left; split < right; split++)
        {
            var leftValues = AchievedValues(numbers, ops, left, split);
            var rightValues = AchievedValues(numbers, ops, split + 1, right);
            AddCombinations(ops[split], leftValues, rightValues, values);
        }

        return values;
    }

    private static void AddCombinations(
        char op, List<long> leftValues, List<long> rightValues, List<long> values)
    {
        foreach (var a in leftValues)
        {
            foreach (var b in rightValues)
            {
                var combined = Combine(op, a, b);
                values.Add(combined);
            }
        }
    }

    private static long Combine(char op, long left, long right) => op switch
    {
        Addition => left + right,
        _ => left * right,
    };

    private static int TotalScore(int[] answers, int correct, IAchievableSet achievable)
    {
        var total = 0;

        foreach (var answer in answers)
        {
            total += ScoreFor(answer, correct, achievable);
        }

        return total;
    }

    private static int ScoreFor(int answer, int correct, IAchievableSet achievable)
    {
        if (answer == correct)
        {
            return CorrectAnswerScore;
        }

        return achievable.Contains(answer) ? AchievableAnswerScore : WrongAnswerScore;
    }

    // The decided question every non-correct answer is graded against: can some full
    // parenthesization of the expression produce this value? The two strategies differ
    // only in what backs the set - the unmemoized arm dedupes its collected values into
    // a BCL HashSet<long>, the memoized arm already holds a HashMap<long,bool> keyed by
    // interval - and grading never has to know which, only that the question is
    // membership.
    private interface IAchievableSet
    {
        // Whether some full parenthesization of the expression produces `value`.
        bool Contains(long value);
    }

    // The baseline's set: the values the unmemoized recursion collected, deduped into
    // a BCL HashSet<long>.
    private sealed class AchievableSetInHashSet(List<long> achieved) : IAchievableSet
    {
        private readonly HashSet<long> _values = new(achieved);

        public bool Contains(long value) => _values.Contains(value);
    }

    // This repo's own HashMap<long,bool>, whose Keys the memoized arm unions over each
    // interval's splits - the same container it already walks to combine the children.
    private sealed class AchievableSetInHashMap(HashMap<long, bool> achieved) : IAchievableSet
    {
        public bool Contains(long value) => achieved.HasKey(value);
    }
}
