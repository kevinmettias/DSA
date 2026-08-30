using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BaseballGame;

// LeetCode 682. Baseball Game: this repo's own Stack<int> (ValidParenthesesTests/
// NextGreaterElementITests precedent) holds the running record of round scores;
// "+"/"D"/"C" each pop/peek/push against it directly instead of indexing a
// hand-managed list.
public sealed partial class BaseballGameTests
{
    [Fact]
    public void CalPoints_ClassicExampleWithAllOperationTypes_ReturnsSumOfValidScores()
    {
        string[] ops = ["5", "2", "C", "D", "+"];

        var total = CalPoints(ops);

        Assert.Equal(30, total);
    }

    [Fact]
    public void CalPoints_OnlyIntegerScores_ReturnsSimpleSum()
    {
        string[] ops = ["1", "2", "3"];

        var total = CalPoints(ops);

        Assert.Equal(6, total);
    }

    [Fact]
    public void CalPoints_NegativeScoreFollowedByDoubleAndSum_TracksSignCorrectly()
    {
        string[] ops = ["-3", "D", "9", "+"];

        var total = CalPoints(ops);

        Assert.Equal(3, total);
    }

    private static int CalPoints(string[] ops)
    {
        var record = new RepoIntStack();

        foreach (var op in ops)
        {
            switch (op)
            {
                case "C":
                    record.TryPop(out _);
                    break;
                case "D":
                    record.TryPeek(out var last);
                    record.Push(last * 2);
                    break;
                case "+":
                    record.TryPop(out var top);
                    record.TryPop(out var second);
                    var sum = top + second;
                    record.Push(second);
                    record.Push(top);
                    record.Push(sum);
                    break;
                default:
                    record.Push(int.Parse(op));
                    break;
            }
        }

        var total = 0;

        while (record.TryPop(out var value))
        {
            total += value;
        }

        return total;
    }
}
