namespace DecisionTree
{
    public class GiniCriterion : ISplitCriterion
    {
        private double CalculateGini(List<WineSample> data)
        {
            if (!data.Any()) return 0.0;

            var totalSamples = data.Count;
            var counts = data.GroupBy(s => s.Cultivar).Select(g => g.Count());

            double gini = 1.0;
            foreach (var count in counts)
            {
                double p = (double)count / totalSamples;
                gini -= p * p;
            }
            return gini;
        }

        public double Calculate(List<WineSample> trueSet, List<WineSample> falseSet)
        {
            int total = trueSet.Count + falseSet.Count;
            double giniTrue = CalculateGini(trueSet);
            double giniFalse = CalculateGini(falseSet);
            return (trueSet.Count * giniTrue + falseSet.Count * giniFalse) / total;
        }
    }
}
