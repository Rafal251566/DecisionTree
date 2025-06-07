namespace DecisionTree
{
    public interface IDecisionNode
    {
        string Predict(WineSample sample);
        void PrintTree(List<WineSample> data, string indent = "", bool isLast = true);
        int CalculateError(List<WineSample> data);
    }
}
