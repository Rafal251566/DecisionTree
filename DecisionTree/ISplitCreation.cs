namespace DecisionTree
{
    public interface ISplitCriterion
    {
        double Calculate(List<WineSample> trueSet, List<WineSample> falseSet);
    }
}
